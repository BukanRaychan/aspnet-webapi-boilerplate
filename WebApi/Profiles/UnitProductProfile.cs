using AutoMapper;
using ProductCatalogAPI.DTOs.UnitProductDtos;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Profiles;

public class UnitProductProfile : Profile
{
    public UnitProductProfile()
    {
        CreateMap<UnitProduct, UnitProductResponseDto>();
        CreateMap<CreateUnitProductDto, UnitProduct>();
        CreateMap<UpdateUnitProductDto, UnitProduct>()
            .ForMember(dest => dest.SerialNumber,
                opt => opt.MapFrom((src, dest) => src.SerialNumber ?? dest.SerialNumber))
            .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom((src, dest) => src.ProductId ?? dest.ProductId));
    }
}