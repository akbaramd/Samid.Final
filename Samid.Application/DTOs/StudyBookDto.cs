namespace Samid.Application.DTOs;

public class StudyBookDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public string Code { get; set; } = default!;
}
