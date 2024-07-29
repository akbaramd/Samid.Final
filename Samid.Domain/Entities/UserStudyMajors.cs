using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities
{
  public class UserStudyMajors
  {
    private UserStudyMajors() { }

    public UserStudyMajors(Guid userId, Guid majorGradeId, Guid academicYearId)
    {
      UserId = userId;
      StudyMajorsId = majorGradeId;
      AcademicYearId = academicYearId;
    }

    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    [Required]
    public Guid  StudyMajorsId { get; private set; }
    public StudyMajors StudyMajors { get; private set; } = default!;

    [Required]
    public Guid AcademicYearId { get; private set; }
    public AcademicYear AcademicYear { get; private set; } = default!;
  }
}
