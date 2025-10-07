namespace Butler.Core.RAG.Models;

public class DocumentChunk(string id, string content, float[] embedding) {
    public string Id { get; } = id;
    public string Content { get; } = content;
    public float[] Embedding { get; } = embedding;
}