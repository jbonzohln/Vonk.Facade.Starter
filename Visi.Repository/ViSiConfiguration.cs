using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Visi.Repository.Models;
using Vonk.Core.Metadata;
using Vonk.Core.Pluggability;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;
using Vonk.Repository.MongoDb;

namespace Visi.Repository;

[VonkConfiguration(240)]
public static class ViSiConfiguration
{
    public static IApplicationBuilder ConfigureApp(this IApplicationBuilder app)
    {
        return app;
    }

    public static IServiceCollection AddViSiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ViSiContext>();
        services.TryAddSingleton<ResourceMapper>();
        services.TryAddContextAware<ISearchRepository, ViSiSearchRepository>(ServiceLifetime.Scoped);
        services.TryAddContextAware<IResourceChangeRepository, ViSiChangeRepository>(ServiceLifetime.Scoped);

        services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
        
        services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
        services.TryAddSingleton<MongoResourceMapper>();
        
        return services;
    }
}