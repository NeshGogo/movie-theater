using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/genders")]
    public class GendersController : ControllerBase
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;

        public GendersController(MovieTheaterDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> Get()
        {
            var genders = await context.Genders.ToListAsync();
            var genderDTOs = mapper.Map<List<GenderDTO>>(genders);
            return genderDTOs;
        }

        [HttpGet("{id:int}", Name = "GetGender")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            var gender = await context.Genders.FindAsync(id);
            if (gender == null) return NotFound();
            var genderDTO = mapper.Map<GenderDTO>(gender);
            return genderDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenderCreateDTO genderCreateDTO)
        {
            var gender = mapper.Map<Gender>(genderCreateDTO);
            await context.Genders.AddAsync(gender);
            await context.SaveChangesAsync();
            var genderDTO = mapper.Map<GenderDTO>(gender);
            return new CreatedAtRouteResult("GetGender", new { id = gender.Id }, genderDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenderCreateDTO genderCreateDTO)
        {
            var gender = mapper.Map<Gender>(genderCreateDTO);
            gender.Id = id;
            context.Genders.Update(gender);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Genders.AnyAsync(g => g.Id == id);
            if (!exists) return NotFound();
            context.Genders.Remove(new Gender { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
