using AutoMapper;
using WebApi.DTOs.AuthDtos;
using WebApi.Models;

namespace WebApi.Profiles;

public class ApplicationUserProfile : Profile
{
    public ApplicationUserProfile()
    {
        CreateMap<ApplicationUser, UserInfoResponseDto>();
    }
}