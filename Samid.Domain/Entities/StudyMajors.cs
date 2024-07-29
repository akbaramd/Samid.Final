using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities;

public class StudyMajors
{
  protected StudyMajors() { }

  public StudyMajors(string title, Guid gradeOfStudyId, StudyGrade studyGrade, Guid fieldOfStudyId, StudyField studyField)
  {
    Title = title;
    StudyGradeId = gradeOfStudyId;
    StudyGrade = studyGrade ?? throw new ArgumentNullException(nameof(studyGrade));
    StudyFieldId = fieldOfStudyId;
    StudyField = studyField ?? throw new ArgumentNullException(nameof(studyField));
  }

  [Key]
  public Guid Id { get; set; }

  public string Title { get; set; } = string.Empty!;
  public Guid StudyGradeId { get; private set; }
  public StudyGrade StudyGrade { get; private set; } = default!;
  public Guid StudyFieldId { get; private set; }
  public StudyField StudyField { get; private set; } = default!;

  public ICollection<StudyBook> StudyBooks { get; set; } = new List<StudyBook>();
}
