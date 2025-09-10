using Butler.Core.RAG.Interfaces;
using Butler.Core.RAG.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Infrastructure.Services
{
    internal sealed class IngestService : IIngestService
    {
        private readonly IRAGRepository _repo;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embed;
        private readonly int _chunkChars;
        private readonly int _chunkOverlap;

        public IngestService(IRAGRepository repo, IEmbeddingGenerator<string, Embedding<float>> embed, IConfiguration config)
        {
            _repo = repo;
            _embed = embed;
            _chunkChars = int.Parse(config["Rag:ChunkChars"] ?? "1000");
            _chunkOverlap = int.Parse(config["Rag:ChunkOverlap"] ?? "150");
        }

        /// <summary>
        /// Ingests a document from the specified file path by reading its content, splitting it into chunks,
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task IngestDocumentAsync(string filePath, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is required", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Document not found", filePath);

            string text = await File.ReadAllTextAsync(filePath, ct);
            int index = 0;

            foreach (string chunk in ChunkText(text, _chunkChars, _chunkOverlap))
            {
                Embedding<float> embedding = await _embed.GenerateAsync(value: chunk, options: null, cancellationToken: ct);
                float[] vector = embedding.Vector.ToArray();

                DocumentChunk documentChunk = new($"{filePath}#chunk-{{index:D3}}", chunk);
                await _repo.InsertChunkAsync(documentChunk, vector, ct);
                index++;
            }

            Console.WriteLine($"Ingested {index} chunks from {filePath}");
        }

        public Task IngestFolderAsync(string folderPath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Splits the input text into chunks of specified size with a given overlap.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        private static IEnumerable<string> ChunkText(string text, int size, int overlap)
        {
            if (string.IsNullOrWhiteSpace(text) || size <= 0)
                yield break;

            int step = Math.Max(1, size - overlap);
            for (int i = 0; i < text.Length; i += step)
            {
                int length = Math.Min(size, text.Length - i);
                yield return text.Substring(i, length);
            }
        }
    }
}
