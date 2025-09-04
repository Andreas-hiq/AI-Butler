using Butler.Core.RAG.Models;

namespace Butler.Core.RAG.Services;

public class Generation {

    public async Task<string> Generate(string query, IReadOnlyList<DocumentChunk> documentChunks) {
        string context = string.Join("\n", documentChunks.Select(chunk => chunk.Text));
        string prompt = $"Answer the question based on the context below.\n\nContext:\n{context}\n\nQuestion: {query}\nAnswer:";
        
        //TODO: Call LLM provider using context and prompt
        return await Task.FromResult($"[FAKE ANSWER to '{query}']");
    }
}