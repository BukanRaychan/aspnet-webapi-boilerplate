using AutoMapper;
using WebApi.DTOs.ProductDtos;
using WebApi.Models;

namespace WebApi.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>();
        CreateMap<UnitProduct, ProductUnitItemDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom((src, dest) => src.Name ?? dest.Name))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom((src, dest) => src.Description ?? dest.Description))
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom((src, dest) => src.Price ?? dest.Price));
    }
}