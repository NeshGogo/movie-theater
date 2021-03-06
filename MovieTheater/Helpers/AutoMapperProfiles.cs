using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MovieTheater.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles( GeometryFactory geometryFactory)
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
            CreateMap<Movie, MovieDetailDTO>()
                .ForMember(x => x.Genders, options => options.MapFrom(MapMovieGenderDetail))
                .ForMember(x => x.Arctors, options => options.MapFrom(MapMovieActorDetail));
            //Cinema
            CreateMap<Cinema, CinemaDTO>()
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Location.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Location.X));
            CreateMap<CinemaDTO, Cinema>()
                .ForMember(x => x.Location, x => x.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));
            CreateMap<CinemaCreateDTO, Cinema>()
                .ForMember(x => x.Location, x => x.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));
            // User
            CreateMap<IdentityUser, UserDTO>();
            // Review
            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.UserName, options => options.MapFrom(x => x.User.UserName));
            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewCreateDTO, Review>();
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
        private List<GenderDTO> MapMovieGenderDetail(Movie movie, MovieDetailDTO movieDetailDTO)
        {
            var result = new List<GenderDTO>();
            if (movie.MovieGenders == null) return result;
            movie.MovieGenders.ForEach(movieGender =>
                result.Add(new GenderDTO { Id = movieGender.GenderId, Name = movieGender.Gender.Name }));
            return result;
        }
        private List<MovieActorDetailDTO> MapMovieActorDetail(Movie movie, MovieDetailDTO movieDetailDTO)
        {
            var result = new List<MovieActorDetailDTO>();
            if (movie.MovieActors == null) return result;
            movie.MovieActors.ForEach(movieActor =>
                result.Add(new MovieActorDetailDTO { 
                    ActorId = movieActor.ActorId, 
                    Name = movieActor.Actor.Name, 
                    Character = movieActor.Character 
                }));
            return result;
        }
    }
}
