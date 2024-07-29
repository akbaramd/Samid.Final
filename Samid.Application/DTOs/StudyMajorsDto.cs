namespace Samid.Application.DTOs;

public class StudyMajorsDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public StudyFieldDto? StudyField { get; set; }
  public StudyGradeDto? StudyGrade { get; set; }

  public ICollection<StudyBookDto> StudyBooks { get; set; } = new List<StudyBookDto>();
}
