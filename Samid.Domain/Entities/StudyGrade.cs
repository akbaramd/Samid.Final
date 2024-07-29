namespace Samid.Domain.Entities
{
  public class StudyGrade
  {
    protected StudyGrade(Guid studyStageId) {
      StudyStageId = studyStageId;
    }

    public StudyGrade(Guid id, string title, Guid studyStageId)
    {
      Id = id;
      Title = title;
      StudyStageId = studyStageId;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public Guid StudyStageId { get; private set; } 
    public StudyStage? StudyStage { get; private set; }

    private readonly List<StudyField> _studyFields = new();
    public IReadOnlyCollection<StudyField> StudyFields => _studyFields.AsReadOnly();

    public void ChangeTitle(string newTitle)
    {
      if (string.IsNullOrWhiteSpace(newTitle))
      {
        throw new ArgumentException("Title cannot be empty or whitespace.", nameof(newTitle));
      }

      Title = newTitle;
    }

    public void AddStudyField(StudyField field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      _studyFields.Add(field);
    }

    public void RemoveStudyField(StudyField field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      _studyFields.Remove(field);
    }
  }
}
