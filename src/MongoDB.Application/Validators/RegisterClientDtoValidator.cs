using FluentValidation;
using MongoDB.Application.DTOs;

namespace MongoDB.Application.Validators;

public class RegisterClientDtoValidator : AbstractValidator<RegisterClientDto>
{
    public RegisterClientDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Name can only contain letters, numbers, underscores, and hyphens");
    }
}