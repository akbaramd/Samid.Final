namespace Samid.Domain.Entities;

public class FieldOfStudy
{
  private readonly List<UserAcademicYear> _academicYears = new();

  // Private fields for encapsulation
  private readonly List<FieldOfStudy> _childs = new();

  private readonly List<GradeFieldOfStudy> _gradeFields = new();

  protected FieldOfStudy() { }

  // Constructor to initialize the entity
  public FieldOfStudy(Guid id, string title, FieldOfStudy? parent = null)
  {
    Id = id;
    Title = title;
    Parent = parent;
    ParentId = parent?.Id;
  }

  // Properties with private setters to ensure encapsulation
  
  public Guid Id { get; set; }
  public string Title { get; private set; } = default!;
  public Guid? ParentId { get; private set; }
  public FieldOfStudy? Parent { get; private set; }

  // Readonly collections to expose children and academic years
  public IReadOnlyCollection<FieldOfStudy> Childs => _childs.AsReadOnly();
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

  // Method to set the parent field of study
  public void SetParent(FieldOfStudy parent)
  {
    if (parent == null)
    {
      throw new ArgumentNullException(nameof(parent));
    }

    Parent = parent;
    ParentId = parent.Id;
  }

  // Method to add a child field of study
  public void AddChild(FieldOfStudy child)
  {
    if (child == null)
    {
      throw new ArgumentNullException(nameof(child));
    }

    if (_childs.Any(c => c.Id == child.Id))
    {
      throw new InvalidOperationException("Child already exists.");
    }

    _childs.Add(child);
  }

  // Method to remove a child field of study
  public void RemoveChild(FieldOfStudy child)
  {
    if (child == null)
    {
      throw new ArgumentNullException(nameof(child));
    }

    _childs.Remove(child);
  }

  // Method to add an academic year
  public void AddAcademicYear(UserAcademicYear userAcademicYear)
  {
    if (userAcademicYear == null)
    {
      throw new ArgumentNullException(nameof(userAcademicYear));
    }

    if (_academicYears.Any(ay => ay.Id == userAcademicYear.Id))
    {
      throw new InvalidOperationException("Academic year already exists.");
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
