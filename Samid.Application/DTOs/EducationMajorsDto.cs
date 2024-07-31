namespace Samid.Application.DTOs;

public class EducationMajorsDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public EducationFieldDto? EducationField { get; set; }
  public EducationGradeDto? EducationGrade { get; set; }

  public ICollection<EducationBookDto> EducationBooks { get; set; } = new List<EducationBookDto>();
}
