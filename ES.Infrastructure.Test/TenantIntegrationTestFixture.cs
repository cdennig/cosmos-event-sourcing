using System;
using System.Collections.Generic;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Projects.Domain;

namespace ES.Infrastructure.Test;

public class TenantIntegrationTestFixture : IDisposable
{
    public CosmosTenantEventsRepository<Guid, Project, Guid, Guid> Repository { get; }
    public Container Container { get; }
    public Guid CurrentId => Guid.Parse("cb996bc3-f663-4042-9a6a-a6a410c32938");
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid UserId => Guid.Parse("c576f6ce-ee4f-4d04-a9d3-63c590d322f1");

    public TenantIntegrationTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var cOpts = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true
            }
        };
        var containers = new List<(string, string)>
        {
            (configuration.GetSection("Cosmos")["Db"], configuration.GetSection("Cosmos")["Container"])
        };
        CosmosClient cosmosClient = CosmosClient.CreateAndInitializeAsync(configuration.GetSection("Cosmos")["Url"],
            configuration.GetSection("Cosmos")["Key"], containers, cOpts).Result;
        Container = cosmosClient.GetContainer(configuration.GetSection("Cosmos")["Db"],
            configuration.GetSection("Cosmos")["Container"]);

        cosmosClient.ReadAccountAsync().GetAwaiter().GetResult();

        var domainEventsFactory = new TenantDomainEventsFactory<Guid, Guid, Guid>();
        domainEventsFactory.Initialize(new List<Type>() { typeof(Project) });
        var loggerFactory = (ILoggerFactory)new LoggerFactory();
        var logger = loggerFactory.CreateLogger<CosmosTenantEventsRepository<Guid, Project, Guid, Guid>>();
        var tarfLogger = loggerFactory.CreateLogger<TenantAggregateRootFactory<Guid, Project, Guid, Guid>>();
        var tenantAggregateRootFactory = new TenantAggregateRootFactory<Guid, Project, Guid, Guid>(tarfLogger);
        Repository =
            new CosmosTenantEventsRepository<Guid, Project, Guid, Guid>(Container, domainEventsFactory,
                tenantAggregateRootFactory, logger);
    }

    public void Dispose()
    {
    }
}