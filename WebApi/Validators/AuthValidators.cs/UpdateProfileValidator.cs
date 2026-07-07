using FluentValidation;
using ProductCatalogAPI.DTOs.AuthDtos;

namespace ProductCatalogAPI.Validators.AuthValidators.cs;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        When(x => x.NewPassword != null, () => {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required to set a new password");

            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long"); 
        });
    }
}