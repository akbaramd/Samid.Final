using System.Security.Claims;
using FastEndpoints;
using FluentValidation;

namespace Samid.Application.DTOs.Activity;

public class StudyActivityRequest
{

  public Guid EducationalBookId { get; set; }
  public Guid UserEducationalMajorId { get; set; }
  public TimeSpan  Duration{ get; set; }
  public string?  Description{ get; set; }
}

public class StudyActivityRequestValidator : Validator<StudyActivityRequest>
{
  public StudyActivityRequestValidator()
  {
    RuleFor(x => x.UserEducationalMajorId)
      .NotEmpty().WithMessage("EducationalMajorId is required");
    
    RuleFor(x => x.EducationalBookId)
      .NotEmpty().WithMessage("EducationalBookId is required");
    
    RuleFor(x => x.Duration)
      .NotEmpty().WithMessage("Duration is required");

  }

}
