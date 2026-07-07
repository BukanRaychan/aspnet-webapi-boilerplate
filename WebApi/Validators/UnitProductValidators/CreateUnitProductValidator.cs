using FluentValidation;
using WebApi.Data;
using WebApi.DTOs.UnitProductDtos;

namespace WebApi.Validators.UnitProductValidators;

public class CreateUnitProductValidator : AbstractValidator<CreateUnitProductDto>
{
    public CreateUnitProductValidator(AppDbContext context)
    {
        RuleFor(x => x.SerialNumber)
            .MaximumLength(50).WithMessage("Serial number cannot exceed 50 characters")
            .Must(sn => context.UnitProducts.All(p => p.SerialNumber != sn))
            .WithMessage("Serial number already exists");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required")
            .Must(id => context.Products.Any(p => p.Id == id))
            .WithMessage("Product with this ID does not exist");
            
    }
}