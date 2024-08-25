using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities
{
  public class StudyActivity
  {
    private StudyActivity() { }

    public StudyActivity(Guid userId, Guid userMajorId, Guid bookId , TimeSpan duration , string? description = null)
    {
      UserId = userId;
      UserEducationUserMajorsId = userMajorId;
      EducationBookId = bookId;
      Duration = duration;
      Description = description;
      DoAt = DateTime.UtcNow;
    }

    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    [Required]
    public Guid  UserEducationUserMajorsId { get; private set; }
    public UserEducationMajors UserEducationMajors { get; private set; } = default!;
    
    [Required]
    public Guid  EducationBookId { get; private set; }
    public EducationBook  EducationBook { get; private set; } = default!;

    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }

    public DateTime DoAt { get; set; }

  }
}
