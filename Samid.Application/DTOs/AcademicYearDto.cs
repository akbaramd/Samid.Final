using Samid.Application.Extensions;

namespace Samid.Application.DTOs;

public class AcademicYearDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }

  public string StartPersianDate => StartDate.ToPersianDate();
  public string EndPersianDate => EndDate.ToPersianDate();
}
