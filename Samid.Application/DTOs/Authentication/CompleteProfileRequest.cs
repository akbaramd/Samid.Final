﻿using FastEndpoints;
using FluentValidation;

namespace Samid.Application.DTOs.Authentication;

public class CompleteProfileRequest
{
  public string PhoneNumber { get; set; } = String.Empty;
  public string FirstName { get; set; } = String.Empty;
  public string LastName { get; set; } = String.Empty;
  public DateTime BirthDate { get; set; }
}


public class CompleteProfileRequestValidator : Validator<CompleteProfileRequest>
{
  public CompleteProfileRequestValidator()
  {
    RuleFor(x => x.PhoneNumber)
      .NotEmpty().WithMessage("Phone number is required");

    RuleFor(x => x.FirstName)
      .NotEmpty().WithMessage("First name is required");

    RuleFor(x => x.LastName)
      .NotEmpty().WithMessage("Last name is required");

    RuleFor(x => x.BirthDate)
      .NotEmpty().WithMessage("Birth date is required")
      .LessThan(DateTime.Now).WithMessage("Birth date cannot be in the future");
  }
}