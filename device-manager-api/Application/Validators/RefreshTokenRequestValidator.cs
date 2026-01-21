using device_manager_api.Application.DTOs.Requests;
using FluentValidation;

namespace device_manager_api.Application.Validators;

/// <summary>
/// Validator for RefreshTokenRequest
/// </summary>
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}
