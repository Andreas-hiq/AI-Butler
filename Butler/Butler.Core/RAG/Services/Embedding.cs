namespace Butler.Core.RAG.Services;

public class Embedding {
    public async Task<float[]> Embed(string text) {
        //TODO: call LLM here
        return Enumerable.Repeat(0.1f, 1536).ToArray(); // TEMPORARY, fake embedding
    }
}