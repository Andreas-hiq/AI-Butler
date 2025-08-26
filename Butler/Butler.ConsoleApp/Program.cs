using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System;
using System.Threading.Tasks;
using Butler.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Butler.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            //DI setup
            ServiceProvider services = new ServiceCollection()
                .AddButlerCore(config)
                .BuildServiceProvider();

            IChatService chat = services.GetRequiredService<IChatService>();

            Console.WriteLine("Welcome to Butler. Type 'exit' to exit chat");

            while (true)
            {
                Console.Write("You> ");

                string? userInput = Console.ReadLine();
                if (userInput is null || userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }

                //Not streamed answer
                // var answer = await chat.GetOnceAsync(userInput);

                //Streamed answer
                Console.Write("Butler> ");
                await foreach (StreamingChatMessageContent token in chat.StreamAnswer(userInput))
                {
                    Console.Write(token.Content);
                }
                Console.WriteLine();
            }
        }
    }
}
