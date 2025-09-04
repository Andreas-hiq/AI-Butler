namespace Butler.Core.RAG.Models;

public class RetrievalResult(DocumentChunk documentChunk, double score) {
    public DocumentChunk DocumentChunk { get; } = documentChunk;
    public double Score { get; } = score;
}