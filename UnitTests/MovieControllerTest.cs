
using AzureProjectMagdalenaGorska.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.ServicesFake;
using WebApp_OpenIDConnect_DotNet.Models;
using WebAppOpenIDConnectDotNet.Controllers;
using Xunit;

namespace UnitTests
{
    public class MovieControllerTest
    {
        private readonly MoviesController _controller;
        private readonly IMovieCosmosService _movieCosmosService;

        public MovieControllerTest()
        {
            _movieCosmosService = new MovieCosmosServiceFake();
            _controller = new MoviesController(_movieCosmosService);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsListOfMoviesAsync()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsType<List<Movie>>(result);
            Assert.Equal(3, result?.Count);
            Assert.False(true);
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsCorrectViewModel()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<List<Movie>>(result?.ViewData.Model);
            var movies = result?.ViewData.Model as List<Movie>;
            Assert.Equal(3, movies.Count);
        }

        [Fact]
        public async Task Details_WhenCalled_ReturnsCorrectViewModel()
        {
            // Arrange
            Movie movie = await getExistingMovie();

            // Act
            var result = await _controller.Details(movie.Id) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<Movie>(result?.ViewData.Model);
            AssertObjectsAreEquivalents(movie, result?.ViewData.Model as Movie);
        }

        [Fact]
        public async Task Details_WhenIdNotExist_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details("not valid id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_WhenCalledWithoutArgs_ReturnsCorrectViewModel()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult?.ViewData.Model);
        }

        [Fact]
        public async Task Create_WhenCalledwithMovieModelPassed_CreatesNewMovie()
        {
            // Arrange
            Movie movie = new Movie() { Id = Guid.NewGuid().ToString(), Director = "anana", Title = "SKJSKSK", Type = "dkdkd" };

            // Act
            var result = await _controller.Create(movie);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnsCorrectViewModel()
        {
            // Arrange
            Movie movie = await getExistingMovie();

            // Act
            var result = await _controller.Edit(movie.Id) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<Movie>(result?.ViewData.Model);
            AssertObjectsAreEquivalents(movie, result?.ViewData.Model as Movie);
        }

        [Fact]
        public async Task Edit_WhenIdNotExist_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit("not valid id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_WhenIdIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_WhenCalledwithMovieModelPassed_RedirectsToIndex()
        {
            // Arrange
            Movie movie = await getExistingMovie();
            movie.Title = "New title";

            // Act
            var result = await _controller.Edit(movie.Id, movie);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnsCorrectViewModel()
        {
            // Arrange
            Movie movie = await getExistingMovie();

            // Act
            var result = await _controller.Delete(movie.Id) as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<Movie>(result?.ViewData.Model);
            AssertObjectsAreEquivalents(movie, result?.ViewData.Model as Movie);
        }

        [Fact]
        public async Task Delete_WhenIdNotExist_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit("not valid id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenCalledwithMovieModelPassed_DeletesMovie()
        {
            // Arrange
            Movie movie = await getExistingMovie();

            // Act
            var result = await _controller.DeleteConfirmed(movie.Id);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.Equal("Index", redirect.ActionName);
        }

        private void AssertObjectsAreEquivalents(Movie first, Movie second)
        {
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Title, second.Title);
            Assert.Equal(first.Type, second.Type);
            Assert.Equal(first.Director, second.Director);
        }

        private async Task<Movie> getExistingMovie()
        {
            var movies = await _movieCosmosService.Get("");
            return movies.First();
        }
    }
}
