using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class MovieFilterDTO
    {
        public int Page { get; set; } = 1;
        public int RecordPerPage { get; set; } = 10;
        public PaginationDTO Pagination
        {
            get
            {
                return new PaginationDTO() { Page = Page, RecordPerPage = RecordPerPage };

            }
        }

        public string Title { get; set; }
        public int GenderId { get; set; }
        public bool AtCinema { get; set; }
        public bool NextPremier { get; set; }
        public string OrderField { get; set; }
        public bool OrderByAsc { get; set; } = true;
    }
}
