using Microsoft.AspNetCore.Identity;
using System;

public class User : IdentityUser<Guid>
{
  public string? FirstName { get; private set; }
  public string? LastName { get; private set; }
  public DateTime? BirthDate { get; private set; }

  // Parameterless constructor for EF Core
  public User() { }

  // Public constructor for use in application code
  public User(string firstName, string lastName, DateTime birthDate)
  {
    Id = Guid.NewGuid();
    FirstName = firstName;
    LastName = lastName;
    BirthDate = birthDate;
  }

  // Domain methods
  public void UpdateName(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }

  public void UpdateBirthDate(DateTime birthDate)
  {
    BirthDate = birthDate;
  }

  // Method to check if the profile is complete
  public bool IsProfileComplete()
  {
    return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && BirthDate != default;
  }
}
