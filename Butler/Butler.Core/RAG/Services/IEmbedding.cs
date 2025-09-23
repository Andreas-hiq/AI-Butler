namespace Butler.Core.RAG.Services;

public interface IEmbedding {
    Task<float[]> Embed(string input);
}