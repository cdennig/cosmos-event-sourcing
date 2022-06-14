using System;
using System.Collections.Generic;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ES.Infrastructure.Test;

public class IntegrationTestFixture : IDisposable
{
    public CosmosEventsRepository<Identity.Domain.User, Guid, Guid> Repository { get; }
    public Container Container { get; }
    public Guid CurrentId => Guid.Parse("ede0ecb7-39ff-42ed-be87-92d7c19240b7");
    public Guid UserId => Guid.Parse("c576f6ce-ee4f-4d04-a9d3-63c590d322f1");

    public IntegrationTestFixture()
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

        var domainEventsFactory = new DomainEventsFactory<Guid, Guid>();
        domainEventsFactory.Initialize(new List<Type> { typeof(Identity.Domain.User) });
        var loggerFactory = (ILoggerFactory)new LoggerFactory();
        var logger = loggerFactory.CreateLogger<CosmosEventsRepository<Identity.Domain.User, Guid, Guid>>();
        var arfLogger = loggerFactory.CreateLogger<AggregateRootFactory<Identity.Domain.User, Guid, Guid>>();
        var aggregateRootFactory = new AggregateRootFactory<Identity.Domain.User, Guid, Guid>(arfLogger);
        Repository =
            new CosmosEventsRepository<Identity.Domain.User, Guid, Guid>(Container, domainEventsFactory,
                aggregateRootFactory, logger);
    }

    public void Dispose()
    {
    }
}