using AutoMapper;
using ProductCatalogAPI.DTOs.AuthDtos;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Profiles;

public class ApplicationUserProfile : Profile
{
    public ApplicationUserProfile()
    {
        CreateMap<ApplicationUser, UserInfoResponseDto>();
    }
}