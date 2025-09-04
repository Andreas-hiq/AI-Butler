using Butler.Core.RAG.Models;

namespace Butler.Core.RAG.Services;

public class VectorStore {
    private readonly List<DocumentChunk> _store = new();
    
    public void Add(DocumentChunk chunk) { 
        _store.Add(chunk);
    }
    /*
     * TODO: Improve naming: what's being searched for. A specific vector? A query?
     */
    public IReadOnlyList<RetrievalResult> Search(float[] queryEmbedding, int topK = 3) {
        return _store
            .Select(documentChunk => new RetrievalResult(documentChunk, CosineSimilarity(queryEmbedding, documentChunk.Embedding)))
            .OrderByDescending(result => result.Score)
            .Take(topK)
            .ToList();
    }
    
    /*
     * TODO: describe "Cosine Similarity"
     */
    private static double CosineSimilarity(float[] vectorOne, float[] vectorTwo) {
        double point = 0, normOne = 0, normTwo = 0;
        
        for (int i = 0; i < vectorOne.Length; i++) {
            point += vectorOne[i] * vectorTwo[i];
            normOne += vectorOne[i] * vectorOne[i];
            normTwo += vectorTwo[i] * vectorTwo[i];
        }
        
        return point / (Math.Sqrt(normOne) * Math.Sqrt(normTwo));
    }
}