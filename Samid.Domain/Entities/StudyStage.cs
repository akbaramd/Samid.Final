namespace Samid.Domain.Entities
{
  public class StudyStage
  {
    protected StudyStage() { }

    public StudyStage(Guid id, string title)
    {
      Id = id;
      Title = title;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;

    private readonly List<StudyGrade> _studyGrades = new();
    public IReadOnlyCollection<StudyGrade> StudyGrades => _studyGrades.AsReadOnly();

    public void ChangeTitle(string newTitle)
    {
      if (string.IsNullOrWhiteSpace(newTitle))
      {
        throw new ArgumentException("Title cannot be empty or whitespace.", nameof(newTitle));
      }

      Title = newTitle;
    }

    public void AddStudyGrade(StudyGrade grade)
    {
      if (grade == null)
      {
        throw new ArgumentNullException(nameof(grade));
      }

      _studyGrades.Add(grade);
    }

    public void RemoveStudyGrade(StudyGrade grade)
    {
      if (grade == null)
      {
        throw new ArgumentNullException(nameof(grade));
      }

      _studyGrades.Remove(grade);
    }
  }
}
