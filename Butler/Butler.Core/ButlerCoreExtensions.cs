using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Butler.Core
{
    public static class ButlerCoreExtensions
    {
        /// <summary>
        /// Reigsters the core services for Butler, including the Ollama chat completion service.
        /// Reads "Ollama:BaseUrl" and "Ollama:ModelId" from the provided configuration.
        /// </summary>
        public static IServiceCollection AddButlerCore(this IServiceCollection services, IConfiguration config)
        {
            Uri baseUrl = new Uri(config["Ollama:BaseUrl"] ?? "http://localhost:11434");
            string modelId = config["Ollama:ModelId"] ?? "gemma3:1b";

            services.AddTransient(_ =>
            {
                return Kernel.CreateBuilder()
                .AddOllamaChatCompletion(modelId, baseUrl)
                .Build();
            });

            services.AddTransient<IChatService, ChatService>();

            return services;
        }
    }
}
