using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core
{
    internal sealed class ChatService : IChatService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;

        public ChatService(Kernel kernel)
        {
            _kernel = kernel;
            _chat = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> AskOnce(string userInput, string? systemPrompt = null)
        {
            ChatHistory history = [];
            history.AddSystemMessage(systemPrompt ?? "You are Butler, a helpful assistant.");
            history.AddUserMessage(userInput);

            ChatMessageContent result = await _chat.GetChatMessageContentAsync(history, kernel: _kernel);
            return result.Content ?? string.Empty;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> StreamAnswer(string userInput, string? systemPrompt = null)
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
