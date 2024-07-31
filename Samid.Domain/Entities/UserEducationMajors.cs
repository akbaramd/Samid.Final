using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities
{
  public class UserEducationMajors
  {
    private UserEducationMajors() { }

    public UserEducationMajors(Guid userId, Guid majorGradeId, Guid academicYearId)
    {
      UserId = userId;
      EducationMajorsId = majorGradeId;
      AcademicYearId = academicYearId;
    }

    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    [Required]
    public Guid  EducationMajorsId { get; private set; }
    public EducationMajors EducationMajors { get; private set; } = default!;

    [Required]
    public Guid AcademicYearId { get; private set; }
    public AcademicYear AcademicYear { get; private set; } = default!;
  }
}
