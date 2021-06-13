using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Helpers;
using NetTopologySuite;

namespace MovieTheater.Test
{
    public class BaseTests
    {
        protected readonly string defaultUserId = "52736a28-633f-496c-9c2e-3d1fb986a9fd";
        protected readonly string defaultUserEmail = "userTest@test.com";

        protected MovieTheaterDbContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MovieTheaterDbContext>()
                .UseInMemoryDatabase(dbName).Options;
            return new MovieTheaterDbContext(options);
        }

        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(options =>
           {
               var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
               options.AddProfile(new AutoMapperProfiles(geometryFactory));
           });
            return config.CreateMapper();
        }

        protected ControllerContext BuildControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, defaultUserEmail),
                new Claim(ClaimTypes.Name, defaultUserEmail),
                new Claim(ClaimTypes.NameIdentifier, defaultUserId),
            }));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
