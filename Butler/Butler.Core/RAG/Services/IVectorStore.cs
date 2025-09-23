using Butler.Core.RAG.Models;

namespace Butler.Core.RAG.Services;

public interface IVectorStore {
    void Add(DocumentChunk chunk);
    IReadOnlyList<RetrievalResult> Search(float[] queryEmbedding);
    
}