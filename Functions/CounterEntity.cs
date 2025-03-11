
using Azure;
using Azure.Data.Tables;

//Table entity class
namespace CloudResume.Function
        {
            public class CounterEntity : ITableEntity
            {
                public string? PartitionKey { get; set; }
                public string? RowKey { get; set; }
                public DateTimeOffset? Timestamp { get; set; }
                public ETag ETag { get; set; }
                public int VisitCounts { get; set; }
            }
        }
       