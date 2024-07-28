using Samid.Domain.Entities;

namespace Samid.Application.DTOs.Authentication;

public class AuthUserProfileResponse
{
  public Guid UserId { get; set; }
  public string? UserName { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; } = string.Empty;
  public string? FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; } = string.Empty;
  public DateTime? BirthDate { get; set; }

  public List<AuthUserProfileAcademicYearsResponse> UserAcademicYears { get; set; }

  public class AuthUserProfileAcademicYearsResponse
  {
    public Guid Id { get; set; }
    public Guid AcademicYearId { get; set; }
    public string AcademicYearTitle { get; set; }
    public Guid GradeFieldOfStudyId { get; set; }
    public string GradeFieldOfStudyTitle { get; set; }
  }

  
}
