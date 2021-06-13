using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTheater.Controllers;
using MovieTheater.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieTheater.DTOs;

namespace MovieTheater.Test.UnitTests
{
    [TestClass]
    public class CinamasControllerTest : BaseTests
    {
        [TestMethod]
        public async Task GetNearBy10KmsOrLess()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                await context.Cinemas.AddRangeAsync(new List<Cinema> {
                    new Cinema{ Name = "Agora", Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))}
                });
                await context.SaveChangesAsync();
            }

            var nearByFilter = new CinemaNearbyFilterDTO { DistanceInKms = 10, Latitude = 18.489156, Longitude = -69.802057 };
            
            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigureAutoMapper();
                var controller = new CinemasController(context, mapper, geometryFactory);
                var response = await controller.Nearby(nearByFilter);
                var results = response.Value;
                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results[0].DistanceInMeters < 10000);
            }
            
        }

    }
}
