using System.Threading.Tasks;
using CloudResume.Function;
using Azure;
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Functions.Tests
{
    [TestFixture]
    public class VisitorCounterTests
    {
        private Mock<ILogger<VisitorCountFunction>> _mockILogger;
        private Mock<TableClient> _mockTableClient;
        private VisitorCountFunction _visitorCountFunction;
        private Mock<HttpRequest> _mockHttpRequest;
        private CounterEntity _counterEntity;
        private string _partitionKey = "testPartition";
        private string _rowKey = "testRow";

        [SetUp]
        public void Setup()
        {
            //Arrange
            _mockILogger = new Mock<ILogger<VisitorCountFunction>>();
            _mockTableClient = new Mock<TableClient>();
            _visitorCountFunction = new VisitorCountFunction(_mockILogger.Object);
            _mockHttpRequest = new Mock<HttpRequest>();
        }

        [Test]
        public async Task ConnectionStringMissing_ReturnBadRequest()
        {
            //Arrange
            Environment.SetEnvironmentVariable("CosmosDBConnectionString", null);
            Environment.SetEnvironmentVariable("TableName", "TestTable");

            //Act
            var result = await _visitorCountFunction.Run(_mockHttpRequest.Object);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task TableNameMissing_ReturnBadRequest()
        {
            //Arrange
            Environment.SetEnvironmentVariable("CosmosDBConnectionString", "TestConnectionString");
            Environment.SetEnvironmentVariable("TableName", null);

            //Act
            var result = await _visitorCountFunction.Run(_mockHttpRequest.Object);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

       /* [Test]
        public async Task VisitCountFunction_ReturnTheVisitCounts()
        {
            //Arrange
            Environment.SetEnvironmentVariable("CosmosDBConnectionString", "TestConnectionString");
            Environment.SetEnvironmentVariable("TableName", "TestTable");
            _counterEntity = new CounterEntity
            {
                PartitionKey = _partitionKey,
                RowKey = _rowKey,
                VisitCounts = 5
            };

            var _mockResponse = new MockResponse<CounterEntity>(_counterEntity);
            
            _mockTableClient.Setup(x => x.GetEntityAsync<CounterEntity>(_partitionKey, _rowKey, default, default)).ReturnsAsync(_mockResponse);


            //Act
            var result = await _visitorCountFunction.Run(_mockHttpRequest.Object);

            //Assert
            Assert.That(result.VisitCounts, Is.EqualTo(6));
        }

        [Test]
        public async Task GetCounterEntity_ReturnsVisitCounts()
        {
            //Arrange
            var expectedVisitCounts = 5;
            _counterEntity = new CounterEntity
            {
                PartitionKey = _partitionKey,
                RowKey = _rowKey,
                VisitCounts = expectedVisitCounts
            };

            // Create a mock of Response that returns CounterEntity
            var _mockResponse = new MockResponse<CounterEntity>(_counterEntity);
            
            _mockTableClient.Setup(x => x.GetEntityAsync<CounterEntity>(_partitionKey, _rowKey, default, default)).ReturnsAsync(_mockResponse);

            //Act
            var result = await _visitorCountFunction.GetCounterEntity(_mockTableClient.Object);

            //Assert
            Assert.That(result.VisitCounts, Is.EqualTo(5));
        }

        private class MockResponse<T> : Response<T>
        {
            private readonly T _value;

            public MockResponse(T value)
            {
                _value = value;
            }

            public override T Value => _value;

             public override Response GetRawResponse()
            {
                throw new System.NotImplementedException();
            }
        } */

    }
}