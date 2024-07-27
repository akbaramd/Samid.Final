namespace Samid.Domain.Entities;

public class GradeFieldOfStudy
{
  public Guid GradeOfStudyId { get; private set; }
  public GradeOfStudy GradeOfStudy { get; private set; } = default!;

  public Guid FieldOfStudyId { get; private set; }
  public FieldOfStudy FieldOfStudy { get; private set; }= default!;

  protected GradeFieldOfStudy() {}
  
  public GradeFieldOfStudy(Guid gradeOfStudyId, GradeOfStudy gradeOfStudy, Guid fieldOfStudyId, FieldOfStudy fieldOfStudy)
  {
    GradeOfStudyId = gradeOfStudyId;
    GradeOfStudy = gradeOfStudy ?? throw new ArgumentNullException(nameof(gradeOfStudy));
    FieldOfStudyId = fieldOfStudyId;
    FieldOfStudy = fieldOfStudy ?? throw new ArgumentNullException(nameof(fieldOfStudy));
  }
}
