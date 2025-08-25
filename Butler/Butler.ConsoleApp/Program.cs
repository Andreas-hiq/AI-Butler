using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System;
using System.Threading.Tasks;

namespace Butler.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var modelId = "gemma3:1b";
            var endpoint = new Uri("http://localhost:11434/");

            var builder = Kernel.CreateBuilder().AddOllamaChatCompletion(modelId, endpoint);

            //Kernel
            Kernel kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            //Enable planning
            OllamaPromptExecutionSettings ollamaPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            //Chat history
            var history = new ChatHistory();

            //Initiate back-and-forth chat
            string? userInput;
            do
            {
                //Get user input
                Console.WriteLine("User> ");
                userInput = Console.ReadLine();

                //Add user input to chat history
                history.AddUserMessage(userInput ?? string.Empty);

                //Get response from the AI model
                var result = await chatCompletionService.GetChatMessageContentAsync(
                    history,
                    executionSettings: ollamaPromptExecutionSettings,
                    kernel: kernel
                    );

                //Print the results
                Console.WriteLine("Butler> " + result);
                //Add the response to chat history
                history.AddMessage(result.Role, result.Content ?? string.Empty);
            } while (userInput is not null);
        }
    }
}
