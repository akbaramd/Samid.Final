namespace Samid.Application.DTOs;

public class UserStudyMajorsDto
{
  public Guid Id { get; set; }
  public AcademicYearDto? AcademicYear { get; set; }
  public StudyMajorsDto? StudyMajors { get; set; }
}
