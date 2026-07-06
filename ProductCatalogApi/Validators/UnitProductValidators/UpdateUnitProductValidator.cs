using FluentValidation;
using ProductCatalogAPI.DTOs.UnitProductDtos;
using ProductCatalogAPI.Data;

namespace ProductCatalogAPI.Validators.UnitProductValidators;

public class UpdateUnitProductValidator : AbstractValidator<UpdateUnitProductDto>
{
    public UpdateUnitProductValidator(AppDbContext context)
    {
        RuleFor(x => x.SerialNumber)
            .MaximumLength(50).WithMessage("Serial number cannot exceed 50 characters");

        RuleFor(x => x.ProductId)
            .Must(id => context.Products.Any(p => p.Id == id))
            .WithMessage("Product with this ID does not exist");

    }
}