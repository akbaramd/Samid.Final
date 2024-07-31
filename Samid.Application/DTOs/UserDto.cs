using Samid.Application.Extensions;

namespace Samid.Application.DTOs;

public class UserDto
{
  public UserDto()
  {
    UserEducationMajors = new List<UserEducationMajorsDto>();
  }

  public Guid Id { get; set; }
  public string? UserName { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; } = string.Empty;
  public string? FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; } = string.Empty;
  public DateTime? BirthDate { get; set; }
  public string? BirthPersianDate => BirthDate?.ToPersianDate();

  public ICollection<UserEducationMajorsDto> UserEducationMajors { get; set; }
}
