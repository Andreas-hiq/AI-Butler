using Butler.Core.RAG.Models;
using Butler.Core.RAG.Services;

namespace Butler.Core.RAG;

public class RagPipeline {
    private readonly Retrieval _retrieval;
    private readonly Generation _generation;

    public RagPipeline(Retrieval retrieval, Generation generation) {
        _retrieval = retrieval;
        _generation = generation;
    }

    public async Task<RagResponse> Run(string query) {
        IReadOnlyList<DocumentChunk> relevantChunks = await _retrieval.GetRelevantChunks(query);
        string response = await _generation.Generate(query, relevantChunks);
        return new RagResponse(query, response, relevantChunks);
    }
}