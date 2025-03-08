
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudResume.Function
{
    public class VisitorCountFunction
    {
        private readonly ILogger<VisitorCountFunction> _logger;
        private readonly string? _connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
        private readonly string? _tableName = Environment.GetEnvironmentVariable("TableName");

        private static readonly string _partitionKey = "Counter";
        private static readonly string _rowKey = "Visits";

        public VisitorCountFunction(ILogger<VisitorCountFunction> logger)
        {
            _logger = logger;
        }

        [Function("VisitorCountFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {

            _logger.LogInformation("Connecting to the Cosmos DB Table API");

            if (string.IsNullOrEmpty(_connectionString))
            {
                return new BadRequestObjectResult("Missing CosmosDBConnectionString");
            }

            //Initialize the table client for table api
            var tableClient = new TableClient(_connectionString, _tableName);

            //create the table if it does not exists
            await tableClient.CreateIfNotExistsAsync();

            //Get the current count of visitors
            CounterEntity entity = await GetCounterEntity(tableClient);

            //Increment the VisitCounts count
            entity.VisitCounts++;

            //update the new count value to the table
            tableClient.UpsertEntity(entity);


            return new OkObjectResult(new { entity.VisitCounts });
        }

        public async Task<CounterEntity> GetCounterEntity(TableClient tableClient)
        {
            try
            {
                return await tableClient.GetEntityAsync<CounterEntity>(partitionKey: _partitionKey, rowKey: _rowKey);
            }
            catch (RequestFailedException)
            {
                //if the entity doesnt exist, create a new one with count = 0
                var newEntity = new CounterEntity
                {
                    PartitionKey = _partitionKey,
                    RowKey = _rowKey,
                    VisitCounts = 0
                };
                await tableClient.AddEntityAsync(newEntity);
                return newEntity;
            }

        }
    }
}

