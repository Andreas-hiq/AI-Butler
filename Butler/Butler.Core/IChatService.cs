using Microsoft.SemanticKernel;

namespace Butler.Core
{
    public interface IChatService
    {
        Task<string> AskOnce(string userInput, string? systemPrompt = null);
        IAsyncEnumerable<StreamingChatMessageContent> GetStreamingResponse(string userInput, string? systemPrompt = null);
    }
}
