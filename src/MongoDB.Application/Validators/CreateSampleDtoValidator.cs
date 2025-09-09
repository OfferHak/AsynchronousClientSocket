using FluentValidation;
using MongoDB.Application.DTOs;

namespace MongoDB.Application.Validators;

public class CreateSampleDtoValidator : AbstractValidator<CreateSampleDto>
{
    public CreateSampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required");

        RuleFor(x => x.Hash)
            .NotEmpty()
            .WithMessage("Hash is required");

        RuleFor(x => x.LC_FC)
            .GreaterThanOrEqualTo(0)
            .WithMessage("LC_FC must be greater than or equal to 0");

        RuleFor(x => x.Sample)
            .NotNull()
            .WithMessage("Sample data is required");

        RuleForEach(x => x.Sample)
            .SetValidator(new CreateSampleDataDtoValidator());
    }
}

public class CreateSampleDataDtoValidator : AbstractValidator<CreateSampleDataDto>
{
    public CreateSampleDataDtoValidator()
    {
        RuleFor(x => x.VahicleDate)
            .NotEmpty()
            .WithMessage("Vehicle Date is required");

        RuleFor(x => x.RecievedDate)
            .NotEmpty()
            .WithMessage("Received Date is required");
    }
}