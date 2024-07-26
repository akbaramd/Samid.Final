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
      .NotEmpty().WithMessage("Phone number is required")
      .Must(BeAValidIranianMobileNumber).WithMessage("Invalid Iranian mobile number")
      .Custom((phoneNumber, context) =>
      {
        // Normalize the phone number by removing the + sign if it exists
        if (phoneNumber.StartsWith("+"))
        {
          phoneNumber = phoneNumber.Substring(1);
        }

        // Ensure the phone number is in the correct format
        if (!phoneNumber.StartsWith("09") || phoneNumber.Length != 11)
        {
          context.AddFailure("Invalid Iranian mobile number format. It should start with '09' and be 11 digits long.");
        }
      });

    RuleFor(x => x.Code)
      .NotEmpty().WithMessage("Verification code is required")
      .Length(6).WithMessage("Verification code must be 6 digits");
  }

  private bool BeAValidIranianMobileNumber(string phoneNumber)
  {
    // Remove the + sign if it exists
    if (phoneNumber.StartsWith("+"))
    {
      phoneNumber = phoneNumber.Substring(1);
    }

    // Check if the phone number starts with 09 and has 11 digits
    return phoneNumber.StartsWith("09") && phoneNumber.Length == 11;
  }
}
