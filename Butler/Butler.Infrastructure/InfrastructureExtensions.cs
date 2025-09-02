using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddButlerInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config["Db:ConnectionString"]!;
            NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(connectionString);
            builder.UseVector();
            NpgsqlDataSource dataSource = builder.Build();

            services.AddSingleton(dataSource);


            //TODO: Add other infrastructure services here
            // e.g services.AddTransient<IRagRepository, RagRepository>(); etc

            return services;
        }
    }
}
