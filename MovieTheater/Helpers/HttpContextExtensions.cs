using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParams<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordPerPage)
        {
            double count = await queryable.CountAsync();
            double pages = Math.Ceiling(count / recordPerPage);
            httpContext.Response.Headers.Add("pages", pages.ToString());
        }
    }
}
