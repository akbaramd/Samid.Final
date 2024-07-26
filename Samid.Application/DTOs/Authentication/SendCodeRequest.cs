using FastEndpoints;
using FluentValidation;
using Samid.Application.DTOs.Authentication;

namespace Samid.Application.DTOs.Authentication
{
    public class SendCodeRequest
    {
        private string phoneNumber = string.Empty;

        public string PhoneNumber
        {
            get => phoneNumber;
            set => phoneNumber = NormalizePhoneNumber(value);
        }

        private string NormalizePhoneNumber(string phoneNumber)
        {
            // Remove the + sign if it exists
            if (phoneNumber.StartsWith("+"))
            {
                phoneNumber = phoneNumber.Substring(1);
            }

            return phoneNumber;
        }
    }
}

public class SendCodeRequestValidator : Validator<SendCodeRequest>
{
    public SendCodeRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Must(BeAValidIranianMobileNumber).WithMessage("Invalid Iranian mobile number format")
            .Custom((phoneNumber, context) =>
            {
                // Ensure the phone number is in the correct format
                if (!phoneNumber.StartsWith("09") || phoneNumber.Length != 11)
                {
                    context.AddFailure("Invalid Iranian mobile number format. It should start with '09' and be 11 digits long.");
                }
            });
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