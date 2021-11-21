using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Projects.Infrastructure.Test
{
    public class TestFixture : IDisposable
    {
        private CosmosClient cosmosClient;
        public Container Container { get; }

        public TestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var cOpts = new CosmosClientOptions();
            cOpts.SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };
            var containers = new List<(string, string)>
            {
                (configuration.GetSection("Cosmos")["Db"], configuration.GetSection("Cosmos")["Container"])
            };
            cosmosClient = CosmosClient.CreateAndInitializeAsync(configuration.GetSection("Cosmos")["Url"],
                configuration.GetSection("Cosmos")["Key"], containers, cOpts).Result;
            Container = cosmosClient.GetContainer(configuration.GetSection("Cosmos")["Db"],
                configuration.GetSection("Cosmos")["Container"]);

            var cx = cosmosClient.ReadAccountAsync().Result;
        }

        public void Dispose()
        {
        }
    }
}