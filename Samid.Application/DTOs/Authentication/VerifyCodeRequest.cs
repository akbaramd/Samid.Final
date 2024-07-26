using FastEndpoints;
using FluentValidation;

namespace Samid.Application.DTOs.Authentication;

public class VerifyCodeRequest
{
  public string PhoneNumber { get; set; } = String.Empty;
  public string Code { get; set; } = String.Empty;
}

public class VerifyCodeRequestValidator : Validator<VerifyCodeRequest>
{
  public VerifyCodeRequestValidator()
  {
    RuleFor(x => x.PhoneNumber)
      .NotEmpty().WithMessage("Phone number is required");

    RuleFor(x => x.Code)
      .NotEmpty().WithMessage("Verification code is required")
      .Length(6).WithMessage("Verification code must be 6 digits");
  }
}