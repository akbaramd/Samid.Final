namespace Samid.Domain.Entities
{
  public class EducationStage
  {
    protected EducationStage() { }

    public EducationStage(Guid id, string title)
    {
      Id = id;
      Title = title;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;

    private readonly List<EducationGrade> _educationGrades = new();
    public IReadOnlyCollection<EducationGrade> EducationGrades => _educationGrades.AsReadOnly();

    public void ChangeTitle(string newTitle)
    {
      if (string.IsNullOrWhiteSpace(newTitle))
      {
        throw new ArgumentException("Title cannot be empty or whitespace.", nameof(newTitle));
      }

      Title = newTitle;
    }

    public void AddEducationGrade(EducationGrade grade)
    {
      if (grade == null)
      {
        throw new ArgumentNullException(nameof(grade));
      }

      _educationGrades.Add(grade);
    }

    public void RemoveEducationGrade(EducationGrade grade)
    {
      if (grade == null)
      {
        throw new ArgumentNullException(nameof(grade));
      }

      _educationGrades.Remove(grade);
    }
  }
}
