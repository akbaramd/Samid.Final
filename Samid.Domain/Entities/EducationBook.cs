using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Samid.Domain.Entities
{
  public class EducationBook
  {
    public EducationBook(Guid id, string title, string code)
    {
      Id = id;
      Title = title;
      Code = code;
    }

    [Key]
    public Guid Id { get; private set; }

    public string Title { get; set; } = default!;
    public string Code { get; set; } = default!;


    public ICollection<EducationMajors> EducationMajors { get; set; } = new List<EducationMajors>();

    public void AddField(EducationMajors field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      EducationMajors.Add(field);
    }

    public void RemoveField(EducationMajors field)
    {
      if (field == null)
      {
        throw new ArgumentNullException(nameof(field));
      }

      EducationMajors.Remove(field);
    }
  }
}
