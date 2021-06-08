using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Helpers
{
    public class MovieExistsAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly MovieTheaterDbContext _dbContext;

        public MovieExistsAttribute(MovieTheaterDbContext context)
        {
            this._dbContext = context;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var movieIdObject = context.HttpContext.Request.RouteValues["MovieId"];
            if (movieIdObject == null) return;
            var movieId = int.Parse(movieIdObject.ToString());
            var MovieExists = await _dbContext.Movies.AnyAsync(m => m.Id == movieId);
            if (!MovieExists) context.Result = new NotFoundResult();
            else await next();
        }
    }
}
