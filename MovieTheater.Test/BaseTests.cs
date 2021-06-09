using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Helpers;
using NetTopologySuite;

namespace MovieTheater.Test
{
    public class BaseTests
    {
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
    }
}
