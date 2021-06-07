using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/cinemas")]
    public class CinemasController : CustomBaseController
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public CinemasController(MovieTheaterDbContext context, IMapper mapper, GeometryFactory geometryFactory)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaDTO>>> Get()
        {
            return await Get<Cinema, CinemaDTO>();
        }

        [HttpGet("{id:int}", Name = "GetCinema")]
        public async Task<ActionResult<CinemaDTO>> Get(int id)
        {
            return await Get<Cinema, CinemaDTO>(id);
        }

        [HttpGet("nearby")]
        public async Task<ActionResult<List<CinemaNearbyDTO>>> Nearby([FromQuery] CinemaNearbyFilterDTO filter)
        {
            var userLocation = geometryFactory.CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));
            return await context.Cinemas
                .Where(c => c.Location.IsWithinDistance(userLocation, filter.DistanceInKms * 1000))
                .OrderBy(c => c.Location.Distance(userLocation))
                .Select(c => new CinemaNearbyDTO
                {
                    Name = c.Name,
                    Id = c.Id,
                    Latitude = c.Location.Y,
                    Longitude = c.Location.X,
                    DistanceInMeters = Math.Round(c.Location.Distance(userLocation))
                }).ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaCreateDTO cinemaCreateDTO)
        {
            return await Post<Cinema, CinemaDTO, CinemaCreateDTO>(cinemaCreateDTO, "GetCinema");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CinemaCreateDTO cinemaCreateDTO)
        {
            return await Put<Cinema, CinemaCreateDTO>(id, cinemaCreateDTO);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<CinemaCreateDTO> patchDocument)
        {
            return await Patch<Cinema, CinemaCreateDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Cinema>(id);
        }
    }
}
