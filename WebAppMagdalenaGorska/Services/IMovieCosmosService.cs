using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Models;

namespace AzureProjectMagdalenaGorska.Services
{
    public interface IMovieCosmosService
    {
        Task<List<Movie>> Get(string sqlCosmosQuery);
        Task<Movie> AddAsync(Movie newMovie);
        Task<Movie> Update(Movie movieToUpdate);
        Task Delete(string id, string type);
    }
}
