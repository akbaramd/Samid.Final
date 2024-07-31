namespace Samid.Application.DTOs;

public class EducationBookDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public string Code { get; set; } = default!;
}
