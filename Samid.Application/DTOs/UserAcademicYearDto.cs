namespace Samid.Application.DTOs;

public class UserAcademicYearDto
{
  public Guid Id { get; set; }
  public AcademicYearDto? AcademicYear { get; set; }
  public GradeFieldOfStudyDto? GradeFieldOfStudy { get; set; }
}
