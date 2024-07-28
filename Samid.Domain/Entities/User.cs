using Microsoft.AspNetCore.Identity;

namespace Samid.Domain.Entities;

public class User : IdentityUser<Guid>
{
  // Parameterless constructor for EF Core
  public User()
  {
    UserAcademicYears = new List<UserAcademicYear>();
  }

  // Public constructor for use in application code
  public User(string firstName, string lastName, DateTime birthDate) : this()
  {
    Id = Guid.NewGuid();
    FirstName = firstName;
    LastName = lastName;
    BirthDate = birthDate;
    VerificationAttempts = 0;
    LastVerificationAttempt = null;
    VerificationFailures = 0;
    LastVerificationFailure = null;
  }

  public string? FirstName { get; private set; }
  public string? LastName { get; private set; }
  public DateTime? BirthDate { get; private set; }
  public int VerificationAttempts { get; private set; }
  public DateTime? LastVerificationAttempt { get; private set; }
  public int VerificationFailures { get; private set; }
  public DateTime? LastVerificationFailure { get; private set; }

  public ICollection<UserAcademicYear> UserAcademicYears { get; private set; }

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
    return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && BirthDate != default && UserAcademicYears.Count != 0 && UserAcademicYears.Any(x=>x.AcademicYear.IsCurrentAcademicYear(DateTime.Now));
  }

  // Methods to manage verification attempts
  public void IncrementVerificationAttempts()
  {
    VerificationAttempts++;
    LastVerificationAttempt = DateTime.UtcNow;
  }

  public void ResetVerificationAttempts()
  {
    VerificationAttempts = 0;
    LastVerificationAttempt = null;
  }

  // Methods to manage verification failures
  public void IncrementVerificationFailures()
  {
    VerificationFailures++;
    LastVerificationFailure = DateTime.UtcNow;
  }

  public void ResetVerificationFailures()
  {
    VerificationFailures = 0;
    LastVerificationFailure = null;
  }
}
