using device_manager_api.Application.DTOs.Requests;
using FluentValidation;

namespace device_manager_api.Application.Validators;

/// <summary>
/// Validator for CreateDeviceRequest
/// </summary>
public class CreateDeviceRequestValidator : AbstractValidator<CreateDeviceRequest>
{
    public CreateDeviceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters");

        RuleFor(x => x.State)
            .IsInEnum().WithMessage("State must be a valid DeviceState value");
    }
}
