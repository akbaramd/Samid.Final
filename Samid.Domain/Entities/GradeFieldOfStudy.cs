using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities;

public class GradeFieldOfStudy
{
  protected GradeFieldOfStudy() { }

  public GradeFieldOfStudy(string title, Guid gradeOfStudyId, GradeOfStudy gradeOfStudy, Guid fieldOfStudyId,
    FieldOfStudy fieldOfStudy)
  {
    Title = title;
    GradeOfStudyId = gradeOfStudyId;
    GradeOfStudy = gradeOfStudy ?? throw new ArgumentNullException(nameof(gradeOfStudy));
    FieldOfStudyId = fieldOfStudyId;
    FieldOfStudy = fieldOfStudy ?? throw new ArgumentNullException(nameof(fieldOfStudy));
  }

  [Key] public Guid Id { get; set; }

  public string Title { get; set; } = string.Empty!;
  public Guid GradeOfStudyId { get; private set; }
  public GradeOfStudy GradeOfStudy { get; private set; } = default!;

  public Guid FieldOfStudyId { get; private set; }
  public FieldOfStudy FieldOfStudy { get; private set; } = default!;
}
