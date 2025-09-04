using Butler.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Butler.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Butler. Type 'exit' to exit chat");
            Console.WriteLine("Please select setup:\nFor Custom built enter '1'\nFor Microsoft setup enter '2'");
            string? setupchoice = Console.ReadLine();

            IChatService chat = setupchoice == "1" ? CustomSetup() : MsSetup();
            
            


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
                //string answer = await chat.AskOnce(userInput);

                //Streamed answer
                Console.Write("Butler> ");
                await foreach (StreamingChatMessageContent token in chat.GetStreamingResponse(userInput))
                {
                    Console.Write(token.Content);
                }
                Console.WriteLine();
            }
            return;

            IChatService CustomSetup() {
                ServiceProvider services = new ServiceCollection()
                    .AddButlerCore()
                    .BuildServiceProvider();
                return services.GetRequiredService<IChatService>();
            }

            IChatService MsSetup() {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                //DI setup
                ServiceProvider services = new ServiceCollection()
                    .AddButlerCore(config)
                    .BuildServiceProvider();

                return services.GetRequiredService<IChatService>();
            }
        }
    }
}
