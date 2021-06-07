using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieTheater.Helpers;
using MovieTheater.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MovieTheater
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            // DbConetext
            services.AddDbContext<MovieTheaterDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MovieTheaterDbConnectionString"), 
                sqlServerOptions => sqlServerOptions.UseNetTopologySuite()));
            //GeometryFactory
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
            // AutoMapper
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton(provider =>
                new MapperConfiguration(config =>
               {
                   var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                   config.AddProfile(new AutoMapperProfiles(geometryFactory));
               }).CreateMapper()
            );
            // AzureStorage
            // services.AddTransient<IFileStorage, FileStorageAzure>();
            //File storage Local
            services.AddTransient<IFileStorage, FileStorageLocal>();

            services.AddHttpContextAccessor();
            services.AddControllers()
                    .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
