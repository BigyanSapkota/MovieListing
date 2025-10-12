using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using AutoMapper;
using Domain.Entities;

namespace Application
{
     public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Movie,MovieDto>().ForMember(dest => dest.PosterUrl, opt => opt.Ignore());
            CreateMap<MovieDto, Movie>().ForMember(dest => dest.PosterUrl, opt => opt.Ignore());


            CreateMap<Movie, CreateMovieDto>().ForMember(dest => dest.PosterUrl, opt => opt.Ignore()); 
            CreateMap<CreateMovieDto, Movie>().ForMember(dest => dest.PosterUrl, opt => opt.Ignore());

            CreateMap<RatingDto, Rating>();
            CreateMap<Rating, RatingDto>();



        }
    }
}
