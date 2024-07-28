using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities;

// Entity for AcademicYear
public class UserAcademicYear
{
  private UserAcademicYear() { } // سازنده بدون پارامتر عمومی

  public UserAcademicYear(Guid userId, Guid gradeFieldOfStudy, Guid academicYearId)
  {
    UserId = userId;
    GradeFieldOfStudyId = gradeFieldOfStudy;
    AcademicYearId = academicYearId;
  }

  [Key] public Guid Id { get; set; } // کلید اصلی


  [Required] public Guid UserId { get; private set; } // کلید خارجی

  public User User { get; private set; } = default!; // خاصیت ناوبری

  [Required] public Guid GradeFieldOfStudyId { get; private set; } // کلید خارجی

  public GradeFieldOfStudy GradeFieldOfStudy { get; private set; } = default!; // خاصیت ناوبری

  [Required] public Guid AcademicYearId { get; private set; } // کلید خارجی

  public AcademicYear AcademicYear { get; private set; } = default!; // خاصیت ناوبری
}
