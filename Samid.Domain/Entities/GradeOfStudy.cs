namespace Samid.Domain.Entities;

public class GradeOfStudy
{
  // Private fields for encapsulation
  private readonly List<AcademicYear> _academicYears = new List<AcademicYear>();

  // Properties with private setters to ensure encapsulation
  public Guid Id { get; private set; }
  public string Title { get; private set; } = default!;

  // Readonly collection to expose academic years
  public IReadOnlyCollection<AcademicYear> AcademicYears => _academicYears.AsReadOnly();
  private readonly List<GradeFieldOfStudy> _gradeFields = new List<GradeFieldOfStudy>();
  public IReadOnlyCollection<GradeFieldOfStudy> GradeFields => _gradeFields.AsReadOnly();
  
  protected GradeOfStudy(){}
  
  // Constructor to initialize the entity
  public GradeOfStudy(Guid id, string title)
  {
    Id = id;
    Title = title;
  }

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
  public void AddAcademicYear(AcademicYear academicYear)
  {
    if (academicYear == null)
    {
      throw new ArgumentNullException(nameof(academicYear));
    }
    _academicYears.Add(academicYear);
  }

  // Method to remove an academic year
  public void RemoveAcademicYear(AcademicYear academicYear)
  {
    if (academicYear == null)
    {
      throw new ArgumentNullException(nameof(academicYear));
    }
    _academicYears.Remove(academicYear);
  }
}
