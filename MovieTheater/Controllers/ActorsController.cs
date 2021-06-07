using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using MovieTheater.Helpers;
using MovieTheater.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/actors")]
    public class ActorsController : CustomBaseController
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "Actors";

        public ActorsController(MovieTheaterDbContext context, IMapper mapper, IFileStorage fileStorage)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            return await Get<Actor, ActorDTO>(pagination);
        }

        [HttpGet("{id:int}", Name = "GetActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreateDTO actorCreateDTO)
        {
            var actor = mapper.Map<Actor>(actorCreateDTO);
            if(actorCreateDTO.PhotoFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreateDTO.PhotoFile.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreateDTO.PhotoFile.FileName);
                    actor.Photo = await fileStorage.SaveFileAsync(content, extension, container, actorCreateDTO.PhotoFile.ContentType);
                }
            }
            await context.Actors.AddAsync(actor);
            await context.SaveChangesAsync();
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return new CreatedAtRouteResult("GetActor", new { id = actorDTO.Id }, actorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreateDTO actorCreateDTO)
        {
            var actor = await context.Actors.FindAsync(id);
            if (actor == null) return NotFound();
            actor = mapper.Map(actorCreateDTO, actor);
            if (actorCreateDTO.PhotoFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreateDTO.PhotoFile.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreateDTO.PhotoFile.FileName);
                    actor.Photo = await fileStorage.EditFileAsync(content, extension, container, actor.Photo, actorCreateDTO.PhotoFile.ContentType);
                }
            }
            context.Actors.Update(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
