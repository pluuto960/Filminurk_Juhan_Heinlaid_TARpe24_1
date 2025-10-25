using System.Runtime.CompilerServices;
using Filminurk.Core.Dto;
using Filminurk.Core.ServiceInterface;
using Filminurk.Data;
using Filminurk.Models.Movies;
using Filminurk.Views.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filminurk.Controllers
{
    public class MoviesController : Controller
    {
        private readonly FilminurkTARpe24Context _context;
        private readonly IMovieServices _movieServices;
        private readonly IFilesServices _filesServices; // piltide lisamiseks vajalik fileservices injection
        public MoviesController
            (
            FilminurkTARpe24Context context, 
            IMovieServices movieServices,
            IFilesServices filesServices // piltide lisamiseks vajalik fileservices injection
            )
        {
            _context = context;
            _movieServices = movieServices;
            _filesServices = filesServices; // piltide lisamiseks vajalik fileservices injection
        }
        public IActionResult Index()
        {
            var result = _context.Movies.Select(x => new MoviesIndexViewModel
            {
                ID = x.ID,
                Title = x.Title,
                FirstPublished = x.FirstPublished,
                CurrentRating = x.CurrentRating,
                Vulgar = x.Vulgar,
                Genre = x.Genre
            });
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            MoviesCreateUpdateViewModel result = new();
            return View("CreateUpdate", result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MoviesCreateUpdateViewModel vm)
        {
            var dto = new MoviesDTO()
            {
                ID = vm.ID,
                Title = vm.Title,
                Description = vm.Description,
                FirstPublished = vm.FirstPublished,
                Director = vm.Director,
                Actors = vm.Actors,
                CurrentRating = vm.CurrentRating,
                Vulgar = vm.Vulgar,
                Genre = vm.Genre,
                IsOnAdultSwim = vm.IsOnAdultSwim,
                EntryCreatedAt = vm.EntryCreatedAt,
                EntryModifiedAt = vm.EntryModifiedAt,
                Files = vm.Files,
                FileToApiDTOs = vm.Images
                .Select(x => new FileToApiDTO
                {
                    ImageID= x.ImageID,
                    FilePath= x.FilePath,
                    MovieID= x.MovieID

                }).ToArray()
            };

            var result = await _movieServices.Create(dto);
            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {            
            var movie = await _movieServices.DetailsAsync(id);
            
            if (movie == null)
            {
                return NotFound();
            }
            var images = await _context.FilesToApi
                .Where(x=> x.MovieID == id)
                .Select(y => new ImageViewModel 
                {
                    FilePath = y.ExistingFilepath,
                    ImageID = id,
                }) .ToArrayAsync();




            var vm = new MoviesCreateUpdateViewModel();
            vm.ID = movie.ID;
            vm.Title = movie.Title;
            vm.Description = movie.Description;
            vm.FirstPublished = movie.FirstPublished;
            vm.Director = movie.Director;
            vm.Actors = movie.Actors;
            vm.CurrentRating = movie.CurrentRating;
            vm.Vulgar = movie.Vulgar;
            vm.Genre = movie.Genre;
            vm.IsOnAdultSwim = movie.IsOnAdultSwim;
            vm.EntryCreatedAt = movie.EntryCreatedAt;
            vm.EntryModifiedAt = movie.EntryModifiedAt;
            vm.Images.AddRange(images);

            return View("CreateUpdate", vm);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _movieServices.DetailsAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            var images= await _context.FilesToApi
                .Where(x=>x.MovieID == id)
                .Select(y => new ImageViewModel
                {
                    FilePath=y.ExistingFilepath,
                    ImageID=y.ImageID,
                }).ToArrayAsync();

            var vm = new MoviesDeleteViewModel();
            vm.ID = movie.ID;
            vm.Title = movie.Title;
            vm.Description = movie.Description;
            vm.FirstPublished = movie.FirstPublished;
            vm.Director = movie.Director;
            vm.Actors = movie.Actors;
            vm.CurrentRating = movie.CurrentRating;
            vm.Vulgar = movie.Vulgar;
            vm.Genre = movie.Genre;
            vm.IsOnAdultSwim = movie.IsOnAdultSwim;
            vm.EntryCreatedAt = movie.EntryCreatedAt;
            vm.EntryModifiedAt = movie.EntryModifiedAt;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var movie = await _movieServices.Delete(id);
            if (movie == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        
    
    }
}
