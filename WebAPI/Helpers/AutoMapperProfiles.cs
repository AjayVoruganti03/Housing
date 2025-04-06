using AutoMapper;
using System;
using WebAPI.Models;
using WebAPI.Dtos;


namespace WebAPI.Helpers{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<City, CityDto>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<City, CityUpdateDto>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    } 
}