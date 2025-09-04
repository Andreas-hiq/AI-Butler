using Butler.Core.RAG;
using Butler.Core.RAG.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Butler.Core
{
    internal sealed class ChatService : IChatService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly RagPipeline _ragPipeline;

        public ChatService(Kernel kernel)
        {
            _kernel = kernel;
            _chat = _kernel.GetRequiredService<IChatCompletionService>();
            _ragPipeline = null!;
        }

        public ChatService(RagPipeline ragPipeline) {
            _ragPipeline = ragPipeline;
            _kernel = null!;
        }

        public async Task<string> AskOnce(string userInput, string? systemPrompt = null)
        {
            ChatHistory history = [];
            history.AddSystemMessage(systemPrompt ?? "You are Butler, a helpful assistant.");
            history.AddUserMessage(userInput);

            ChatMessageContent result = await _chat.GetChatMessageContentAsync(history, kernel: _kernel);
            return result.Content ?? string.Empty;
        }

        public async Task<string> AskOnce(string query) {
            RagResponse ragResponse = await _ragPipeline.Run(query);
            return ragResponse.Answer;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingResponse(string userInput, string? systemPrompt = null)
        {
            ChatHistory history = [];
            history.AddSystemMessage(systemPrompt ?? "You are Butler, a helpful assistant.");
            history.AddUserMessage(userInput);

            await foreach (StreamingChatMessageContent token in _chat.GetStreamingChatMessageContentsAsync(history, kernel: _kernel))
            {
                yield return token;
            }
        }
    }
}
