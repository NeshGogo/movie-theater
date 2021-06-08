using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using MovieTheater.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly MovieTheaterDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(MovieTheaterDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            var entityDTOs = mapper.Map<List<TDTO>>(entities);
            return entityDTOs;
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();
            return mapper.Map<TDTO>(entity);
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO pagination) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(pagination, queryable);
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO pagination, IQueryable<TEntity> queryable) where TEntity : class
        {
            await HttpContext.InsertPaginationParams(queryable, pagination.RecordPerPage);
            var entities = await queryable.Pagination(pagination).ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult> Post<TEntity, TDTO, TCreateDTO>(TCreateDTO createDTO, string routerName) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(createDTO);
            await context.Set<TEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
            var dto = mapper.Map<TDTO>(entity);
            return new CreatedAtRouteResult(routerName, new { id = entity.Id }, dto);
        }

        protected async Task<ActionResult> Put<TEntity, TUpdateDTO>(int id, TUpdateDTO updateDTO) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(updateDTO);
            entity.Id = id;
            context.Set<TEntity>().Update(entity);
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Patch<TEntity, TPatchDTO>(int id, JsonPatchDocument<TPatchDTO> patchDocument) 
            where TEntity : class, IId
            where TPatchDTO : class
        {
            if (patchDocument == null) return BadRequest();
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return NotFound();
            var patchDTO = mapper.Map<TPatchDTO>(entity);
            patchDocument.ApplyTo(patchDTO, ModelState);
            var isValid = TryValidateModel(patchDTO);
            if (!isValid) return BadRequest(ModelState);
            mapper.Map(patchDTO, entity);
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var exists = await context.Set<TEntity>().AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            context.Set<TEntity>().Remove(new TEntity() { Id = id});
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
