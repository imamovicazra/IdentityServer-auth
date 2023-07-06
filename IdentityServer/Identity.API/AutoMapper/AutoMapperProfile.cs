using AutoMapper;
using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Identity.Model.Entities;

namespace Identity.API.AutoMapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserRequest>()
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.UserName))
                .ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserResponse>()
                .ForMember(dest => dest.IsEmailConfirmed, opts => opts.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEmailConfirmed, opts => opts.MapFrom(src => src.EmailConfirmed))
                .ReverseMap();
        }
    }
}
