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
                .ForMember(a => a.Poster, options => options.Ignore())
                .ForMember(x => x.MovieGenders, options => options.MapFrom(MapMovieGenders))
                .ForMember(x => x.MovieActors, options => options.MapFrom(MapMovieActors));


            CreateMap<Movie, MoviePatchDTO>().ReverseMap();
        }

        private List<MovieGender> MapMovieGenders(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MovieGender>();
            if (movieCreateDTO.GenderIds == null) return result;
            movieCreateDTO.GenderIds.ForEach(id => result.Add(new MovieGender { GenderId = id }));
            return result;
        }
        private List<MovieActor> MapMovieActors(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MovieActor>();
            if (movieCreateDTO.GenderIds == null) return result;
            movieCreateDTO.Actors.ForEach(actor => 
                result.Add(new MovieActor { ActorId = actor.ActorId, Character = actor.Character }));
            return result;
        } 
    }
}
