using AutoMapper;
using ProductCatalogAPI.DTOs.ProductDtos;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>();
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