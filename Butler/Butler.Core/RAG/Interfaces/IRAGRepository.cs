using Butler.Core.RAG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core.RAG.Interfaces
{
    public interface IRAGRepository
    {
        Task InsertChunkAsync(DocumentChunk chunk, float[] embedding, CancellationToken ct = default);

        Task<IReadOnlyList<RetrievalResult>> SearchAsync(float[] queryEmbedding, int topK, CancellationToken ct = default);
    }
}
