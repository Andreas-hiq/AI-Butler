using Butler.Core;
using Butler.Core.RAG.Interfaces;
using Butler.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Npgsql;

namespace Butler.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            //DI setup
            ServiceProvider services = new ServiceCollection()
                .AddButlerCore(config)
                .AddButlerInfrastructure(config)
                .BuildServiceProvider();

            #region Datasource test
            //Datasource test
            NpgsqlDataSource ds = services.GetRequiredService<NpgsqlDataSource>();

            await using (var cmd = ds.CreateCommand("SELECT 1"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"Datasource test returned: {reader.GetInt32(0)}"); //should write 1
                }
            }
            #endregion

            var ingestService = services.GetRequiredService<IIngestService>();
            var RAGService = services.GetRequiredService<IRAGSearchService>();
            var chatService = services.GetRequiredService<IChatService>();

            Console.WriteLine("Type 'ingest <folder>' or a question. 'exit' quits Butler");

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
                //string answer = await chat.AskOnce(userInput);

                //Streamed answer
                Console.Write("Butler> ");
                await foreach (StreamingChatMessageContent token in chat.GetStreamingResponse(userInput))
                {
                    Console.Write(token.Content);
                }
                Console.WriteLine();
            }
        }
    }
}
