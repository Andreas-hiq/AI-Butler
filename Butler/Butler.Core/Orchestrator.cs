using Butler.Core.RAG.Interfaces;
using Butler.Core.RAG.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core
{
    public sealed class Orchestrator
    {
        private readonly IRAGSearchService _searchService;
        private readonly IChatService _chatService;
        private readonly ILogger<Orchestrator>? _logger;

        private const int DefaultTopK = 5;
        private const double RelevanceThreshold = 0.30; // Filter out results below this relevance score
        private const int MaxContextChars = 2500; // Protects against prompt bloat

        public Orchestrator(IRAGSearchService searchService, IChatService chatService, ILogger<Orchestrator>? logger = null)
        {
            _searchService = searchService;
            _chatService = chatService;
            _logger = logger;
        }

        public async Task<(string Answer, IReadOnlyList<(string Source, double Score)> Sources)>
            AskWithRagAsync(string query, int topK = DefaultTopK, CancellationToken ct = default)
        {
            // Retrieve relevant documents
            var hits = await _searchService.SearchAsync(query, topK, ct);
            var filteredHits = hits.Where(h => h.Score >= RelevanceThreshold).ToList();

            // Build context from hits (truncate if necessary)
            var context = BuildContext(filteredHits);
            if (string.IsNullOrWhiteSpace(context))
            {
                _logger?.LogInformation("No relevant context found for query: {query}", query);
                return ("I can't find it in my dossier.", Array.Empty<(string, double)>());
            }

            // Create prompt
            var prompt = $"""
            SYSTEM:
            You are Butler. Answer only from GIVEN CONTEXT.
                If the answer is missing: say "I can't find it in my dossier."

            CONTEXT:
            {context}

            QUERY:
            {query}
            """;

            // Get answer from chat service
            var answer = await _chatService.AskOnce(prompt);

            // Prepare sources for return
            var sources = filteredHits
                .Select(hit => (hit.DocumentChunk.Source, (double)hit.Score))
                .ToList()
                .AsReadOnly();

            _logger?.LogInformation("RAG: {Count} hits (top score {Top:0.00}, low {Low:0.00})",
                filteredHits.Count, filteredHits.Max(hit => hit.Score), filteredHits.Min(hit => hit.Score));

            return (answer, sources);

        }

        private static string BuildContext(IEnumerable<RetrievalResult> filteredHits)
        {
            var sb = new StringBuilder();
            const string separator = "\n---\n";

            foreach (var hit in filteredHits)
            {
                var content = hit.DocumentChunk.Content;
                int newLength = sb.Length + (sb.Length > 0 ? separator.Length : 0) + content.Length;

                if (newLength > MaxContextChars)
                {
                    // If we haven't added anything yet, add as much as fits to ensure at least some context
                    if (sb.Length == 0)
                    {
                        sb.Append(content.Substring(0, Math.Min(content.Length, MaxContextChars)));
                    }
                    // If we already have context, stop here to avoid a partial chunk
                    break;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(content);
            }

            return sb.ToString();
        }
    }
}
