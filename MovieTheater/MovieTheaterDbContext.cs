using Microsoft.EntityFrameworkCore;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater
{
    public class MovieTheaterDbContext : DbContext
    {
        public MovieTheaterDbContext( DbContextOptions options) : base(options)
        {

        }

        public DbSet<Gender> Genders { get; set; }
        public DbSet<Actor> Actors { get; set; }
    }
}
