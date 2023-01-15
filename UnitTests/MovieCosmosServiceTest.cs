using AzureProjectMagdalenaGorska.Services;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Models;
using Xunit;

namespace UnitTests
{
    public class MovieCosmosServiceTest
    {
        private readonly MovieCosmosService _service;
        private const string COSMOS_GET_QUERY = "sample query";
        private readonly Movie _movie;
        private readonly List<Movie> _moviesList;
        public MovieCosmosServiceTest()
        {
            var container = new Mock<Container>();
            var client = new Mock<CosmosClient>();

            _moviesList = new List<Movie>
            {
                new Movie(){ Id = Guid.NewGuid().ToString(), Director = "anana", Title= "SKJSKSK", Type="dkdkd"},
                new Movie(){ Id = Guid.NewGuid().ToString(), Director = "anana", Title= "SKJSKSK", Type="dkdkd"},
                new Movie(){ Id = Guid.NewGuid().ToString(), Director = "anana", Title= "SKJSKSK", Type="dkdkd"}
            };

            _movie = new Movie() { Id = Guid.NewGuid().ToString(), Director = "new", Title = "new", Type = "new" };

            var feedResponseMock = new Mock<FeedResponse<Movie>>();
            feedResponseMock.Setup(x => x.GetEnumerator()).Returns(_moviesList.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<Movie>>();
            feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));

            var responseMock = new Mock<ItemResponse<Movie>>();
            responseMock.Setup(p => p.Resource).Returns(_movie);

            container
                .Setup(c => c.GetItemQueryIterator<Movie>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            container.Setup(c => c.CreateItemAsync(
                _movie,
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseMock.Object)
                .Verifiable();

            container.Setup(c => c.UpsertItemAsync(
                _movie,
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseMock.Object)
                .Verifiable();

            container.Setup(c => c.DeleteItemAsync<Movie>(
                _movie.Id,
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseMock.Object)
                .Verifiable(); ;

            string dbName = "dbName";
            string collectionName = "collectionName";

            client.Setup(x => x.GetContainer(dbName, collectionName)).Returns(container.Object);

            _service = new MovieCosmosService(client.Object, dbName, collectionName);
        }

        [Fact]
        public async Task Get_ShouldReturnListOfMovies()
        {
            //Act
            var response = await _service.Get(COSMOS_GET_QUERY);

            //Assert
            Assert.Equal(response.Count, _moviesList.Count);
            Assert.True(response.SequenceEqual(_moviesList));
        }

        [Fact]
        public async Task AddAsync_ShouldReturnCreatedElement()
        {
            //Act
            var response = await _service.AddAsync(_movie);

            //Assert
            Assert.Equal(response, _movie);
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedElement()
        {
            //Act
            var response = await _service.Update(_movie);

            //Assert
            Assert.Equal(response, _movie);
        }

        [Fact]
        public async Task Delete_ShouldNotThrow()
        {
            //Act
            await _service.Delete(_movie.Id, _movie.Type);
        }
    }
}
