using FluentValidation;
using MongoDB.Application.DTOs;

namespace MongoDB.Application.Validators;

public class CreateSampleDtoValidator : AbstractValidator<CreateSampleDto>
{
    public CreateSampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.Hash)
            .GreaterThan(0).WithMessage("Hash must be greater than 0");

        RuleFor(x => x.LC_FC)
            .GreaterThanOrEqualTo(0).WithMessage("LC_FC must be greater than or equal to 0");

        RuleFor(x => x.Sample)
            .NotNull().WithMessage("Sample data is required")
            .Must(samples => samples.Count > 0).WithMessage("At least one sample data is required");

        RuleForEach(x => x.Sample)
            .SetValidator(new SampleDataDtoValidator());
    }
}

public class UpdateSampleDtoValidator : AbstractValidator<UpdateSampleDto>
{
    public UpdateSampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.Hash)
            .GreaterThan(0).WithMessage("Hash must be greater than 0");

        RuleFor(x => x.LC_FC)
            .GreaterThanOrEqualTo(0).WithMessage("LC_FC must be greater than or equal to 0");

        RuleFor(x => x.Sample)
            .NotNull().WithMessage("Sample data is required")
            .Must(samples => samples.Count > 0).WithMessage("At least one sample data is required");

        RuleForEach(x => x.Sample)
            .SetValidator(new SampleDataDtoValidator());
    }
}

public class SampleDataDtoValidator : AbstractValidator<SampleDataDto>
{
    public SampleDataDtoValidator()
    {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data is required");

        RuleFor(x => x.VehicleDate)
            .NotEmpty().WithMessage("Vehicle Date is required");

        RuleFor(x => x.ReceivedDate)
            .NotEmpty().WithMessage("Received Date is required");
    }
}