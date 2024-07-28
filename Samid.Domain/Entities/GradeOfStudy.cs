namespace Samid.Domain.Entities;

public class GradeOfStudy
{
  // Private fields for encapsulation
  private readonly List<UserAcademicYear> _academicYears = new();
  private readonly List<GradeFieldOfStudy> _gradeFields = new();

  protected GradeOfStudy() { }

  // Constructor to initialize the entity
  public GradeOfStudy(Guid id, string title)
  {
    Id = id;
    Title = title;
  }

  // Properties with private setters to ensure encapsulation
  public Guid Id { get; private set; }
  public string Title { get; private set; } = default!;

  // Readonly collection to expose academic years
  public IReadOnlyCollection<UserAcademicYear> AcademicYears => _academicYears.AsReadOnly();
  public IReadOnlyCollection<GradeFieldOfStudy> GradeFields => _gradeFields.AsReadOnly();

  // Method to change the title
  public void ChangeTitle(string newTitle)
  {
    if (string.IsNullOrWhiteSpace(newTitle))
    {
      throw new ArgumentException("Title cannot be empty or whitespace.", nameof(newTitle));
    }

    Title = newTitle;
  }

  // Method to add an academic year
  public void AddAcademicYear(UserAcademicYear userAcademicYear)
  {
    if (userAcademicYear == null)
    {
      throw new ArgumentNullException(nameof(userAcademicYear));
    }

    _academicYears.Add(userAcademicYear);
  }

  // Method to remove an academic year
  public void RemoveAcademicYear(UserAcademicYear userAcademicYear)
  {
    if (userAcademicYear == null)
    {
      throw new ArgumentNullException(nameof(userAcademicYear));
    }

    _academicYears.Remove(userAcademicYear);
  }
}
