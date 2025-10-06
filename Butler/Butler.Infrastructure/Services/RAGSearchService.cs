using Butler.Core.RAG.Interfaces;
using Butler.Core.RAG.Models;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Infrastructure.Services
{
    internal sealed class RAGSearchService : IRAGSearchService
    {
        private readonly IRAGRepository _repo;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embed;

        public RAGSearchService(IRAGRepository repo, IEmbeddingGenerator<string, Embedding<float>> embed)
        {
            _repo = repo;
            _embed = embed;
        }
        public async Task<IReadOnlyList<RetrievalResult>> SearchAsync(string query, int topK, CancellationToken ct = default)
        {
            Embedding<float> queryEmbed = await _embed.GenerateAsync(value: query, options: null, cancellationToken: ct);
            float[] queryVector = queryEmbed.Vector.ToArray();
            return await _repo.SearchAsync(queryVector, topK, ct);
        }
    }
}
