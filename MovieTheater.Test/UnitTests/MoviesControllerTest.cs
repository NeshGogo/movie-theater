using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieTheater.Controllers;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test.UnitTests
{
    [TestClass]
    public class MoviesControllerTest : BaseTests
    {
        [TestMethod]
        public async Task GetMoviesFilteredByTitle()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            
            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { Title = "Movie 1" };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;
            
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(movieFilterDTO.Title, results[0].Title);
        }

        [TestMethod]
        public async Task GetMoviesFilteredByAtCinema()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { AtCinema = true };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Movie in theaters", results[0].Title);
            Assert.IsTrue(results[0].AtCinema);
        }

        [TestMethod]
        public async Task GetMoviesFilteredByNextPremiere()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { NextPremier = true };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Not Premiered", results[0].Title);
        }

        [TestMethod]
        public async Task GetMoviesFilteredByGender()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            var genderId = context.Genders.Select(g => g.Id).First();

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { GenderId = genderId };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Movie with gender", results[0].Title);
        }

        [TestMethod]
        public async Task GetMoviesFilteredOrderByTitleAscending()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { OrderField = "Title", OrderByAsc = true };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDb = await context2.Movies.OrderBy(m => m.Title).ToListAsync();
            Assert.AreEqual(moviesDb.Count, results.Count);
            for (int i = 0; i < moviesDb.Count; i++)
            {
                Assert.AreEqual(moviesDb[i].Id, results[i].Id);
            }
        }

        [TestMethod]
        public async Task GetMoviesFilteredOrderByTitleDescending()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { OrderField = "Title", OrderByAsc = false };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDb = await context2.Movies.OrderByDescending(m => m.Title).ToListAsync();
            Assert.AreEqual(moviesDb.Count, results.Count);
            for (int i = 0; i < moviesDb.Count; i++)
            {
                Assert.AreEqual(moviesDb[i].Id, results[i].Id);
            }
        }

        [TestMethod]
        public async Task GetMoviesFilteredOrderByWorngField ()
        {
            var dbName = await CreateTestData();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            var mock = new Mock<ILogger<MoviesController>>();

            var controller = new MoviesController(context, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var movieFilterDTO = new MovieFilterDTO { OrderField = "test", OrderByAsc = true };
            var response = await controller.Filter(movieFilterDTO);
            var results = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDb = await context2.Movies.ToListAsync();
            Assert.AreEqual(moviesDb.Count, results.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        private async Task<string> CreateTestData()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var gender = new Gender { Name = "Gender 1" };
            var movies = new List<Movie>
            {
                new Movie { Title = "Movie 1", PremiereDate = new DateTime(2010,1,1), AtCinema = false },
                new Movie { Title = "Not Premiered", PremiereDate = DateTime.Today.AddDays(2), AtCinema = false },
                new Movie { Title = "Movie in theaters", PremiereDate = DateTime.Today.AddDays(-2), AtCinema = true },
            };
            var movieWithGender = new Movie
            {
                Title = "Movie with gender",
                PremiereDate = new DateTime(2010, 1, 1),
                AtCinema = false
            };
            movies.Add(movieWithGender);
            
            await context.Genders.AddAsync(gender);
            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
            await context.MovieGenders.AddAsync(new MovieGender { GenderId = gender.Id, MovieId = movieWithGender.Id });
            await context.SaveChangesAsync();
            
            return dbName;
        }
    }
}
