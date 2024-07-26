using FastEndpoints;
using FluentValidation;

namespace Samid.Application.DTOs.Authentication;

public class SendCodeRequest
{
  public string PhoneNumber { get; set; } = string.Empty;
}

public class SendCodeRequestValidator : Validator<SendCodeRequest>
{
  public SendCodeRequestValidator()
  {
    RuleFor(x => x.PhoneNumber)
      .NotEmpty().WithMessage("Phone number is required")
      .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");
  }
}