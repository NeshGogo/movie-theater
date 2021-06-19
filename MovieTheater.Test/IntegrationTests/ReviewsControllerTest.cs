using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test.IntegrationTests
{
    [TestClass]
    public class ReviewsControllerTest : BaseTests
    {
        private readonly string url = "/api/movies/1/reviews";

        [TestMethod]
        public async Task GetReviewsReturn404WhenMovieNotExists()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        [TestMethod]
        public async Task GetReviews()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var context = BuildContext(dbName);
            await context.Movies.AddAsync(new Movie { AtCinema = false, Title = "Movie 1", PremiereDate = DateTime.Now });
            await context.SaveChangesAsync();
            
            var client = factory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var reviews =  JsonConvert.DeserializeObject<List<ReviewDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(0, reviews.Count);
        }
    }
}
