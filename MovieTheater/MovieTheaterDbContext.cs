using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTheater
{
    public class MovieTheaterDbContext : IdentityDbContext
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
            modelBuilder.Entity<MovieCinema>()
                .HasKey(ma => new { ma.CinemaId, ma.MovieId });

            SeedData(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            var roleAdminId = Guid.NewGuid().ToString();
            var userAdminId = Guid.NewGuid().ToString();
            var roleAdmin = new IdentityRole { Id = roleAdminId, Name = "Admin", NormalizedName = "Admin" };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var userAdminEmail = "neshgogo@hotmail.com";
            var userAdmin = new IdentityUser 
            { 
                UserName = userAdminEmail, 
                Id = userAdminId, 
                Email = userAdminEmail,
                NormalizedEmail = userAdminEmail, 
                NormalizedUserName = userAdminEmail ,
                PasswordHash = passwordHasher.HashPassword(null, "123456")
            };
            /*modelBuilder.Entity<IdentityUser>().HasData(userAdmin);
            modelBuilder.Entity<IdentityRole>().HasData(roleAdmin);
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = userAdminId,
                    ClaimValue = "Admin"
                });*/

            var adventure = new Gender { Id = 4, Name = "Adventure" };
            var animation = new Gender { Id = 5, Name = "Animation" };
            var suspense = new Gender { Id = 6, Name = "Suspense" };
            var romance = new Gender { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Gender>().HasData(new List<Gender>
            {
                adventure, animation, suspense, romance
            });

            var jimCarrey = new Actor { Id = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Actor { Id = 6, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 04, 4) };
            var chrisEvans = new Actor { Id = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>().HasData(new List<Actor>
            {
                jimCarrey, robertDowney, chrisEvans
            });

            var endGame = new Movie { 
                Id = 2, 
                Title = "Avengers: Endgame",
                AtCinema = true,
                PremiereDate = new DateTime(2019, 04, 26) 
            };

            var sonic = new Movie
            {
                Id = 4,
                Title = "Sonic the Hedgehog",
                AtCinema = false,
                PremiereDate = new DateTime(2020, 02, 28)
            };

            var iw = new Movie
            {
                Id = 3,
                Title = "Avengers: Infinity Wars",
                AtCinema = false,
                PremiereDate = new DateTime(2019, 04, 26)
            };

            var emma = new Movie
            {
                Id = 5,
                Title = "Emma",
                AtCinema = false,
                PremiereDate = new DateTime(2020, 02, 21)
            };

            var wonderWoman = new Movie
            {
                Id = 6,
                Title = "Wonder woman 1984",
                AtCinema = false,
                PremiereDate = new DateTime(2020, 08, 14)
            };

            modelBuilder.Entity<Movie>().HasData(new List<Movie>
            {
                endGame, iw, sonic, emma, wonderWoman,
            });

            modelBuilder.Entity<MovieGender>().HasData(
                new List<MovieGender>()
                {
                    new MovieGender(){MovieId = endGame.Id, GenderId = suspense.Id},
                    new MovieGender(){MovieId = endGame.Id, GenderId = adventure.Id},
                    new MovieGender(){MovieId = iw.Id, GenderId = suspense.Id},
                    new MovieGender(){MovieId = iw.Id, GenderId = adventure.Id},
                    new MovieGender(){MovieId = sonic.Id, GenderId = adventure.Id},
                    new MovieGender(){MovieId = emma.Id, GenderId = suspense.Id},
                    new MovieGender(){MovieId = emma.Id, GenderId = romance.Id},
                    new MovieGender(){MovieId = wonderWoman.Id, GenderId = suspense.Id},
                    new MovieGender(){MovieId = wonderWoman.Id, GenderId = adventure.Id},
                });

            modelBuilder.Entity<MovieActor>().HasData(
                new List<MovieActor>()
                {
                    new MovieActor(){MovieId = endGame.Id, ActorId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MovieActor(){MovieId = endGame.Id, ActorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MovieActor(){MovieId = iw.Id, ActorId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MovieActor(){MovieId = iw.Id, ActorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MovieActor(){MovieId = sonic.Id, ActorId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
                });
            
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<Cinema>()
               .HasData(new List<Cinema>
               {
                    //new Cinema{Id = 1, Name = "Agora", Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
                    new Cinema{Id = 4, Name = "Sambil", Location = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
                    new Cinema{Id = 5, Name = "Megacentro", Location = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
                    new Cinema{Id = 6, Name = "Village East Cinema", Location = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
               });
        }

        public DbSet<Gender> Genders { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieGender> MovieGenders { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MovieCinema> MovieCinemas { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
