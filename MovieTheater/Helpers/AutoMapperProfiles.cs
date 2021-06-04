using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MovieTheater.DTOs;
using MovieTheater.Entities;

namespace MovieTheater.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Gender
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<GenderCreateDTO, Gender>();
            // Actor
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreateDTO, Actor>()
                .ForMember(a => a.Photo, options => options.Ignore());
            CreateMap<Actor, ActorPatchDTO>().ReverseMap();
            //Movie
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreateDTO, Movie>()
                .ForMember(a => a.Poster, options => options.Ignore());
            CreateMap<Movie, MoviePatchDTO>().ReverseMap();
        }
    }
}
