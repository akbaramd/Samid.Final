﻿using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using FastEndpoints;
using FluentValidation;

namespace Samid.Application.DTOs.Authentication;

public class AuthCompleteProfileRequest
{
  [JsonIgnore] [Ignore] public string UserId { get; set; } = string.Empty;

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public DateTime BirthDate { get; set; }
  public Guid GradeOfStudyId { get; set; }
  public Guid? FieldOfStudyId { get; set; }
}

public class CompleteProfileRequestValidator : Validator<AuthCompleteProfileRequest>
{
  public CompleteProfileRequestValidator()
  {
    RuleFor(x => x.FirstName)
      .NotEmpty().WithMessage("First name is required");

    RuleFor(x => x.LastName)
      .NotEmpty().WithMessage("Last name is required");
    RuleFor(x => x.GradeOfStudyId)
      .NotEmpty().WithMessage("GradeOfStudyId  is required");
    RuleFor(x => x.BirthDate)
      .NotEmpty().WithMessage("Birth date is required")
      .LessThan(DateTime.Now).WithMessage("Birth date cannot be in the future");
  }
}
