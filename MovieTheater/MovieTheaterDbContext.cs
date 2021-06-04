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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.ActorId, ma.MovieId });
            modelBuilder.Entity<MovieGender>()
                .HasKey(ma => new { ma.GenderId, ma.MovieId });
            
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieGender> MovieGenders { get; set; }
    }
}
