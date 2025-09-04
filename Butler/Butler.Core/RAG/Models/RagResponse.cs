namespace Butler.Core.RAG.Models;

public class RagResponse(string query, string answer, IReadOnlyList<DocumentChunk> sources) {
    public string Query { get; } = query;
    public string Answer { get; } = answer;
    public IReadOnlyList<DocumentChunk> Sources { get; } = sources;
}