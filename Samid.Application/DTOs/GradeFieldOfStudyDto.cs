namespace Samid.Application.DTOs;

public class GradeFieldOfStudyDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public FieldOfStudyDto? FieldOfStudy { get; set; }
  public GradeOfStudyDto? GradeOfStudy { get; set; }
}
