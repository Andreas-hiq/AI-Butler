using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core
{
    public interface IChatService
    {
        Task<string> AskOnce(string userInput, string? systemPrompt = null);
        IAsyncEnumerable<StreamingChatMessageContent> GetStreamingResponse(string userInput, string? systemPrompt = null);
    }
}
