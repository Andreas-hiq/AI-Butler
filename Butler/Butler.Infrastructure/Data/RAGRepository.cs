using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Butler.Core.RAG.Interfaces;
using Butler.Core.RAG.Models;
using Npgsql;
using Pgvector;

namespace Butler.Infrastructure.Data
{
    internal sealed class RAGRepository : IRAGRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public RAGRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Inserts a document chunk and its embedding into the database.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="embedding"></param>
        /// <param name="ct"></param>
        public async Task InsertChunkAsync(DocumentChunk chunk, float[] embedding, CancellationToken ct = default)
        {
            const string sql = @"
                INSERT INTO rag_chunks (id, source, content, embedding)
                VALUES (@id, @source, @content, @embedding)
                ";

            await using NpgsqlCommand cmd = _dataSource.CreateCommand(sql);
            cmd.Parameters.AddWithValue("id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("source", chunk.Source);
            cmd.Parameters.AddWithValue("content", chunk.Content);
            cmd.Parameters.AddWithValue("embedding", new Vector(embedding));
            await cmd.ExecuteNonQueryAsync(ct);
        }

        /// <summary>
        /// Searches for the most relevant document chunks based on the provided query embedding.
        /// </summary>
        /// <remarks>The relevance score is a value between 0 and 1, where higher values indicate greater
        /// similarity to the query embedding.</remarks>
        /// <param name="queryEmbedding">A vector representing the query embedding used to calculate similarity with stored document chunks.</param>
        /// <param name="topK">The maximum number of results to return, ordered by relevance.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Defaults to <see
        /// langword="default"/>.</param>
        /// <returns>A read-only list of <see cref="RetrievalResult"/> objects, each containing a document chunk and its
        /// relevance score. The list is ordered by descending relevance, with the most relevant result first. If no
        /// matches are found, the list will be empty.</returns>
        public async Task<IReadOnlyList<RetrievalResult>> SearchAsync(float[] queryEmbedding, int topK, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT source, content, (1 - (embedding <=>) @query AS score)
                FROM rag_chunks
                ORDER BY embedding <=> @query
                LIMIT @topK
                ";
            List<RetrievalResult> list = new();

            await using NpgsqlCommand cmd = _dataSource.CreateCommand(sql);
            cmd.Parameters.AddWithValue("query", new Vector(queryEmbedding));
            cmd.Parameters.AddWithValue("topK", topK);

            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                DocumentChunk chunk = new(
                    Source: reader.GetString(0),
                    Content: reader.GetString(1)
                );
                float score = reader.GetFloat(2); // Score is between 0 and 1, the higher the better
                list.Add(new RetrievalResult(chunk, score));
            }

            return list;
        }
    }
}
