using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using MovieTheater.Helpers;
using MovieTheater.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MovieTheater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "Movies";
        private readonly ILogger<MoviesController> logger;

        public MoviesController(
            MovieTheaterDbContext context, 
            IMapper mapper, 
            IFileStorage fileStorage,
            ILogger<MoviesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<MovieIndexDTO>> Get()
        {
            var top = 5;
            var today = DateTime.Today;

            var nextPremiere = await context.Movies
                .Where(m => m.PremiereDate > today)
                .OrderBy(m => m.PremiereDate)
                .Take(top)
                .ToListAsync();

            var atCinema = await context.Movies
                .Where(m => m.AtCinema)
                .Take(top)
                .ToListAsync();
            var result = new MovieIndexDTO
            {
                AtCinema = mapper.Map<List<MovieDTO>>(atCinema),
                FuturePremiere = mapper.Map<List<MovieDTO>>(nextPremiere),
            };
            return result;
        }
        
        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MovieFilterDTO movieFilter)
        {
            var moviesQueryable = context.Movies.AsQueryable();
            if (!String.IsNullOrEmpty(movieFilter.Title))
            {
                moviesQueryable = moviesQueryable.Where(m => m.Title.Contains(movieFilter.Title));
            }

            if (movieFilter.AtCinema)
            {
                moviesQueryable = moviesQueryable.Where(m => m.AtCinema);
            }

            if (movieFilter.NextPremier)
            {
                moviesQueryable = moviesQueryable.Where(m => m.PremiereDate > DateTime.Today);
            }

            if (movieFilter.GenderId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(m => m.MovieGenders.Select( mg => mg.GenderId)
                    .Contains(movieFilter.GenderId));
            }

            if (!string.IsNullOrEmpty(movieFilter.OrderField))
            {
                var typeOrder = movieFilter.OrderByAsc ? "ascending" : "descending";
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{movieFilter.OrderField} {typeOrder}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
                
            }

            await HttpContext.InsertPaginationParams(moviesQueryable, movieFilter.RecordPerPage);
            var movieDTOS = await moviesQueryable.Pagination(movieFilter.Pagination).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movieDTOS);
        }

        [HttpGet("{id:int}", Name = "GetMovie")]
        public async Task<ActionResult<MovieDetailDTO>> Get(int id)
        {
            var movie = await context.Movies
                .Include(m => m.MovieActors)
                .ThenInclude( m => m.Actor)
                .Include( m => m.MovieGenders)
                .ThenInclude( mg => mg.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            movie.MovieActors = movie.MovieActors.OrderBy(ma => ma.Order).ToList();
            return mapper.Map<MovieDetailDTO>(movie);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm]MovieCreateDTO movieCreateDTO)
        {
            var movie = mapper.Map<Movie>(movieCreateDTO);
            if (movieCreateDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);
                    movie.Poster = await fileStorage.SaveFileAsync(content, extension, container, movieCreateDTO.Poster.ContentType);
                }
            }
            AsignOrderToActors(movie);
            await context.Movies.AddAsync(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovie", new { id = movieDTO.Id }, movieDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreateDTO movieCreateDTO)
        {
            var movie = await context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieGenders)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            movie = mapper.Map(movieCreateDTO, movie);
            if (movieCreateDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);
                    movie.Poster = await fileStorage.SaveFileAsync(content, extension, container, movieCreateDTO.Poster.ContentType);
                }
            }
            AsignOrderToActors(movie);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> jsonPatchDocument)
        {
            if (jsonPatchDocument == null) return BadRequest();
            var movie = await context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            var moviePatchDTO = mapper.Map<MoviePatchDTO>(movie);
            jsonPatchDocument.ApplyTo(moviePatchDTO, ModelState);
            var isValid = TryValidateModel(moviePatchDTO);
            if (!isValid) return BadRequest(ModelState);
            mapper.Map(moviePatchDTO, movie);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id){
            var exists = await context.Movies.AnyAsync(a => a.Id == id);
            if (!exists) return NotFound();
            context.Movies.Remove(new Movie { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignOrderToActors(Movie movie)
        {
            var length = movie.MovieActors.Count;
            if ( length > 0)
            {
                for (int i = 0; i < movie.MovieActors.Count; i++)
                {
                    movie.MovieActors[i].Order = i;
                }
            }
        }
    }
}