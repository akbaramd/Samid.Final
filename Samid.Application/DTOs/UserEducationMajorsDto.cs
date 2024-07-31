namespace Samid.Application.DTOs;

public class UserEducationMajorsDto
{
  public Guid Id { get; set; }
  public AcademicYearDto? AcademicYear { get; set; }
  public EducationMajorsDto? EducationMajors { get; set; }
}
