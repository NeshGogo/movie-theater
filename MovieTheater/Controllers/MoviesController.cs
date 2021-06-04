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
using MovieTheater.Services;

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

        public MoviesController(MovieTheaterDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }

        [HttpGet("{id:int}", Name = "GetMovie")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return mapper.Map<MovieDTO>(movie);
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