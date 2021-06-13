using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ReviewsControllerTest : BaseTests
    {
        [TestMethod]
        public async Task UserCannotAddTwoReviewsToTheSameMovie()
        {
            var dbName = Guid.NewGuid().ToString();
            await InitData(dbName);
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            var movieId = context.Movies.Select(m => m.Id).First();
            var review = new Review { MovieId = movieId, Score = 5, UserId = defaultUserId, Comment = "good" };
            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
            var context2 = BuildContext(dbName);
            var controller = new ReviewsController(context2, mapper);
            controller.ControllerContext = BuildControllerContext();
            var reviewCreate = new ReviewCreateDTO { Score = 5, Comment = "Great" };
            var response = await controller.Post(movieId, reviewCreate);
            var result = response as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task CreateReview()
        {
            var dbName = Guid.NewGuid().ToString();
            await InitData(dbName);
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            var movieId = context.Movies.Select(m => m.Id).First();
            var controller = new ReviewsController(context, mapper);
            controller.ControllerContext = BuildControllerContext();
            var reviewCreate = new ReviewCreateDTO { Score = 5, Comment = "Great" };
            var response = await controller.Post(movieId, reviewCreate);
            var result = response as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            var context2 = BuildContext(dbName);
            var review = await context2.Reviews.FirstOrDefaultAsync();
            Assert.IsNotNull(review);
            Assert.AreEqual(defaultUserId, review.UserId);
        }

        private async Task InitData(string dbName)
        {
            var context = BuildContext(dbName);
            await context.Movies.AddAsync(new Movie
            {
                Title = "Emma",
                AtCinema = false,
                PremiereDate = new DateTime(2020, 02, 21)
            });
            await context.SaveChangesAsync();
        }
    }
}
