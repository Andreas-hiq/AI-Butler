namespace Butler.Core.RAG.Models;

public class DocumentChunk(string id, string text, float[] embedding) {
    public string Id { get; } = id;
    public string Text { get; } = text;
    public float[] Embedding { get; } = embedding;
}