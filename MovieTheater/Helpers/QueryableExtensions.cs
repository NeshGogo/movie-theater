using MovieTheater.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            var result = queryable.Skip((pagination.Page - 1) * pagination.RecordPerPage).Take(pagination.RecordPerPage);
            return result;
        }
    }
}
