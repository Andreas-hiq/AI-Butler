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
                .AddSingleton<IConfiguration>(config)
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

            IIngestService ingestService = services.GetRequiredService<IIngestService>();
            //IRAGSearchService RAGService = services.GetRequiredService<IRAGSearchService>();
            //IChatService chatService = services.GetRequiredService<IChatService>();
            Orchestrator orchestrator = services.GetRequiredService<Orchestrator>();

            Console.WriteLine("Type 'ingest <folder>' or a question. 'exit' quits Butler");

            string? userInput;
            while ((userInput = Console.ReadLine()) is not null)
            {
                if (userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (userInput.StartsWith("ingest ", StringComparison.OrdinalIgnoreCase))
                {
                    string path = userInput.Substring(7).Trim();
                    await ingestService.IngestFolderAsync(path);
                    Console.WriteLine("Ingest done");
                    continue;
                }

                //IReadOnlyList<Core.RAG.Models.RetrievalResult> hits = await RAGService.SearchAsync(userInput, topK: 5);
                //string context = string.Join("\n---\n", hits.Select(h => h.DocumentChunk.Content));
                //string prompt = $@"SYSTEM:
                //                    You are Butler. Answer only from GIVEN CONTEXT.
                //                    If the answer is missing: say 'I can't find it in my dossier.'
                                    
                //                    CONTEXT:
                //                    {context}
                //                    QUERY:
                //                    {userInput}";
                //string answer = await chatService.AskOnce(prompt);
                //Console.WriteLine($"\nButler> {answer}");
                //Console.WriteLine("Sources:");
                //foreach (Core.RAG.Models.RetrievalResult hit in hits)
                //    Console.WriteLine(" - " + hit.DocumentChunk.Source);

                var (answer, sources) = await orchestrator.AskWithRagAsync(userInput);
                Console.WriteLine($"\nButler> {answer}\nSources:");
                foreach (var (Source, Score) in sources)
                    Console.WriteLine($" - {Source}     ({Score:0.00})");

            }

            //IChatService chat = services.GetRequiredService<IChatService>();

            //Console.WriteLine("Welcome to Butler. Type 'exit' to exit chat");

            //while (true)
            //{
            //    Console.Write("You> ");

            //    string? userInput = Console.ReadLine();
            //    if (userInput is null || userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            //    {
            //        break;
            //    }
            //    if (string.IsNullOrWhiteSpace(userInput))
            //    {
            //        continue;
            //    }

            //    //Not streamed answer
            //    //string answer = await chat.AskOnce(userInput);

            //    //Streamed answer
            //    Console.Write("Butler> ");
            //    await foreach (StreamingChatMessageContent token in chat.GetStreamingResponse(userInput))
            //    {
            //        Console.Write(token.Content);
            //    }
            //    Console.WriteLine();
            //}
        }
    }
}
