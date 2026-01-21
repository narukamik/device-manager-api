using device_manager_api.Application.DTOs.Requests;
using FluentValidation;

namespace device_manager_api.Application.Validators;

/// <summary>
/// Validator for ChangePasswordRequest
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .Must(PasswordValidator.Validate)
            .WithMessage(PasswordValidator.GetRequirements());
    }
}
