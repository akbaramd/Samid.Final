namespace Samid.Application.DTOs;

public class FieldOfStudyDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public FieldOfStudyDto? Parent { get; set; }
}
