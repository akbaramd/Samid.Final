namespace Samid.Domain.Entities
{
  public class EducationGrade
  {
    protected EducationGrade(Guid educationStageId) {
      EducationStageId = educationStageId;
    }

    public EducationGrade(Guid id, string title, Guid educationStageId)
    {
      Id = id;
      Title = title;
      EducationStageId = educationStageId;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public Guid EducationStageId { get; private set; } 
    public EducationStage? EducationStage { get; private set; }

    private readonly List<EducationField> _educationFields = new();
    public IReadOnlyCollection<EducationField> EducationFields => _educationFields.AsReadOnly();

    public void ChangeTitle(string newTitle)
    {
      if (string.IsNullOrWhiteSpace(newTitle))
      {
        throw new ArgumentException("Title cannot be empty or whitespace.", nameof(newTitle));
      }

      Title = newTitle;
    }

    public void AddEducationField(EducationField field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      _educationFields.Add(field);
    }

    public void RemoveEducationField(EducationField field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      _educationFields.Remove(field);
    }
  }
}
