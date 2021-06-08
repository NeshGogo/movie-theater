using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using MovieTheater.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [Route("api/movies/{MovieId:int}/reviews")]
    [ApiController]
    [ServiceFilter(typeof(MovieExistsAttribute))]
    public class ReviewsController : CustomBaseController
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;

        public ReviewsController(MovieTheaterDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int movieId,[FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Reviews
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId);
            return await Get<Review, ReviewDTO>(pagination, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int movieId, [FromBody] ReviewCreateDTO reviewCreate)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewExists = await context.Reviews.AnyAsync(r => r.MovieId == movieId && r.UserId == userId);
            if (reviewExists) return BadRequest("The user already wrote a review in this movie.");
            var review = mapper.Map<Review>(reviewCreate);
            review.MovieId = movieId;
            review.UserId = userId;
            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int movieId, int reviewId, [FromBody] ReviewCreateDTO reviewCreate)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewDb = await context.Reviews.FindAsync(reviewId);
            if (reviewDb == null) return NotFound();
            if (reviewDb.UserId != userId) return BadRequest("Your are not allowed to edit this review.");
            mapper.Map(reviewCreate, reviewDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int movieId, int reviewId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewDb = await context.Reviews.FindAsync(reviewId);
            if (reviewDb == null) return NotFound();
            if (reviewDb.UserId != userId) return BadRequest("Your are not allowed to delete this review.");
            context.Reviews.Remove(reviewDb);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
