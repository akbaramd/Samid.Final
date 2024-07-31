using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities;

public class EducationMajors
{
  protected EducationMajors() { }

  public EducationMajors(string title, Guid gradeOfEducationId, EducationGrade educationGrade, Guid fieldOfEducationId, EducationField educationField)
  {
    Title = title;
    EducationGradeId = gradeOfEducationId;
    EducationGrade = educationGrade ?? throw new ArgumentNullException(nameof(educationGrade));
    EducationFieldId = fieldOfEducationId;
    EducationField = educationField ?? throw new ArgumentNullException(nameof(educationField));
  }

  [Key]
  public Guid Id { get; set; }

  public string Title { get; set; } = string.Empty!;
  public Guid EducationGradeId { get; private set; }
  public EducationGrade EducationGrade { get; private set; } = default!;
  public Guid EducationFieldId { get; private set; }
  public EducationField EducationField { get; private set; } = default!;

  public ICollection<EducationBook> EducationBooks { get; set; } = new List<EducationBook>();
}
