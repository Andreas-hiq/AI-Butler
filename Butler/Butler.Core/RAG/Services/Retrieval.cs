using Butler.Core.RAG.Models;

namespace Butler.Core.RAG.Services;

public class Retrieval {
    private readonly IEmbedding _embedding;
    private readonly IVectorStore _vectorStore;

    public Retrieval(IEmbedding embedding, IVectorStore vectorStore) {
        _embedding = embedding;
        _vectorStore = vectorStore;
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetRelevantChunks(string query) {
        float[] queryEmbedding = await _embedding.Embed(query);
        IReadOnlyList<RetrievalResult> results = _vectorStore.Search(queryEmbedding);
        return results.Select(retrievalResults => retrievalResults.DocumentChunk).ToList();
    }
}