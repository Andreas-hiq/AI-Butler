using Microsoft.Extensions.DependencyInjection;

namespace Butler.Core;

public static class ButlerCoreExtensionsCustom {
    public static IServiceCollection AddButlerCore(this IServiceCollection services) {
        services.AddSingleton<RAG.Services.Embedding>();
        services.AddSingleton<RAG.Services.VectorStore>();
        services.AddSingleton<RAG.Services.Retrieval>();
        services.AddSingleton<RAG.Services.Generation>();
        services.AddSingleton<RAG.RagPipeline>();
        services.AddSingleton<IChatService, ChatService>();
        
        return services;
    }
}