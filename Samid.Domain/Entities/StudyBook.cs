using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities
{
  public class StudyBook
  {
    public StudyBook(Guid id, string title, string code)
    {
      Id = id;
      Title = title;
      Code = code;
    }

    [Key]
    public Guid Id { get; private set; }

    public string Title { get; set; } = default!;
    public string Code { get; set; } = default!;


    public ICollection<StudyMajors> StudyMajors { get; set; } = new List<StudyMajors>();

    public void AddField(StudyMajors field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      StudyMajors.Add(field);
    }

    public void RemoveField(StudyMajors field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      StudyMajors.Remove(field);
    }
  }
}
