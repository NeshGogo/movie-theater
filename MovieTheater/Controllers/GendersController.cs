using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/genders")]
    public class GendersController : CustomBaseController
    {
        public GendersController(MovieTheaterDbContext context, IMapper mapper)
            : base(context, mapper){}

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> Get()
        {
            return await Get<Gender, GenderDTO>();
        }

        [HttpGet("{id:int}", Name = "GetGender")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            return await Get<Gender, GenderDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenderCreateDTO genderCreateDTO)
        {
            return await Post<Gender, GenderDTO, GenderCreateDTO>(genderCreateDTO, "GetGender");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenderCreateDTO genderCreateDTO)
        {
            return await Put<Gender, GenderCreateDTO>(id, genderCreateDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Gender>(id);
        }

    }
}
