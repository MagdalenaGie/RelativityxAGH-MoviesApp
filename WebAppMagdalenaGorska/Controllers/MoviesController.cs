using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp_OpenIDConnect_DotNet.Models;
using AzureProjectMagdalenaGorska.Services;

namespace WebAppOpenIDConnectDotNet.Controllers
{
    public class MoviesController : Controller
    {

        public readonly IMovieCosmosService _movieCosmosService;

        public MoviesController(IMovieCosmosService movieCosmosService)
        {
            _movieCosmosService = movieCosmosService;
        }

        public async Task<List<Movie>> GetAll()
        {
            var sqlCosmosQuery = "Select * from c";
            return await _movieCosmosService.Get(sqlCosmosQuery);
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await this.GetAll());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await this.GetAll();
            var movie = movies.Find(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Director,Type")] Movie newMovie)
        {
            if (ModelState.IsValid)
            {
                newMovie.Id = Guid.NewGuid().ToString();
                await _movieCosmosService.AddAsync(newMovie);
                return RedirectToAction(nameof(Index));
            }
            return View(newMovie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movies = await this.GetAll();
            var movie = movies.Find(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Director,Type")] Movie movieToUpdate)
        {
            if (id != movieToUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _movieCosmosService.Update(movieToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!MovieExists(movieToUpdate.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movieToUpdate);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await this.GetAll();
            var movie = movies.Find(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movies = await this.GetAll();
            var movie = movies.Find(m => m.Id == id);

            await _movieCosmosService.Delete(id, movie.Type);
            return RedirectToAction(nameof(Index));
        }

        //private bool MovieExists(string id)
        //{
        //    var sqlCosmosQuery = "Select * from c";
        //    var movies = await _movieCosmosService.Get(sqlCosmosQuery);
        //    var movie = movies.Find(m => m.Id == id);
        //    return _context.Movie.Any(e => e.Id == id);
        //}
    }
}
