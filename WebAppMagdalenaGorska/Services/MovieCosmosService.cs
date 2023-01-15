﻿using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Models;

namespace AzureProjectMagdalenaGorska.Services
{
    public class MovieCosmosService : IMovieCosmosService
    {
        private readonly Container _container;
        public MovieCosmosService(CosmosClient cosmosClient,
        string databaseName,
        string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<List<Movie>> Get(string sqlCosmosQuery)
        {
            var query = _container.GetItemQueryIterator<Movie>(new QueryDefinition(sqlCosmosQuery));

            List<Movie> result = new List<Movie>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<Movie> AddAsync(Movie newMovie)
        {
            var item = await _container.CreateItemAsync(newMovie, new PartitionKey(newMovie.Type));
            return item;
        }

        public async Task<Movie> Update(Movie movieToUpdate)
        {
            var item = await _container.UpsertItemAsync(movieToUpdate, new PartitionKey(movieToUpdate.Type));
            return item;
        }

        public async Task Delete(string id, string type)
        {
            await _container.DeleteItemAsync<Movie>(id, new PartitionKey(type));
        }
    }
}
