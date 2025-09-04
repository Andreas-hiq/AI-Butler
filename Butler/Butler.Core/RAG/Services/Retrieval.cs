using Butler.Core.RAG.Models;

namespace Butler.Core.RAG.Services;

public class Retrieval {
    private readonly Embedding _embedding;
    private readonly VectorStore _vectorStore;

    public Retrieval(Embedding embedding, VectorStore vectorStore) {
        _embedding = embedding;
        _vectorStore = vectorStore;
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetRelevantChunks(string query) {
        float[] queryEmbedding = await _embedding.Embed(query);
        IReadOnlyList<RetrievalResult> results = _vectorStore.Search(queryEmbedding);
        return results.Select(retrievalResults => retrievalResults.DocumentChunk).ToList();
    }
}