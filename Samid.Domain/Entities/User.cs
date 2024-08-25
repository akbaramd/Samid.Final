using Microsoft.AspNetCore.Identity;
using Samid.Domain.Constants;

namespace Samid.Domain.Entities;

/// <summary>
///   Represents a user in the application, extending IdentityUser with additional properties and methods.
/// </summary>
public class User : IdentityUser<Guid>
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="User" /> class for EF Core.
  /// </summary>
  public User()
  {
    UserEducationMajors = new List<UserEducationMajors>();
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="User" /> class for use in application code.
  /// </summary>
  /// <param name="firstName">The first name of the user.</param>
  /// <param name="lastName">The last name of the user.</param>
  /// <param name="birthDate">The birth date of the user.</param>
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

  /// <summary>
  ///   Gets the first name of the user.
  /// </summary>
  public string? FirstName { get; private set; }

  /// <summary>
  ///   Gets the last name of the user.
  /// </summary>
  public string? LastName { get; private set; }

  /// <summary>
  ///   Gets the birth date of the user.
  /// </summary>
  public DateTime? BirthDate { get; private set; }

  /// <summary>
  ///   Gets the number of verification attempts made by the user.
  /// </summary>
  public int VerificationAttempts { get; private set; }

  /// <summary>
  ///   Gets the timestamp of the last verification attempt.
  /// </summary>
  public DateTime? LastVerificationAttempt { get; private set; }

  /// <summary>
  ///   Gets the number of verification failures encountered by the user.
  /// </summary>
  public int VerificationFailures { get; private set; }

  /// <summary>
  ///   Gets the timestamp of the last verification failure.
  /// </summary>
  public DateTime? LastVerificationFailure { get; private set; }

  /// <summary>
  ///   Gets the collection of education majors associated with the user.
  /// </summary>
  public ICollection<UserEducationMajors> UserEducationMajors { get; }

  /// <summary>
  ///   Updates the user's name.
  /// </summary>
  /// <param name="firstName">The new first name of the user.</param>
  /// <param name="lastName">The new last name of the user.</param>
  public void UpdateName(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }

  /// <summary>
  ///   Updates the user's birth date.
  /// </summary>
  /// <param name="birthDate">The new birth date of the user.</param>
  public void UpdateBirthDate(DateTime birthDate)
  {
    BirthDate = birthDate;
  }

  /// <summary>
  ///   Determines whether the user's profile is complete.
  /// </summary>
  /// <returns>True if the profile is complete; otherwise, false.</returns>
  public bool IsProfileComplete()
  {
    return !string.IsNullOrEmpty(FirstName) &&
           !string.IsNullOrEmpty(LastName) &&
           BirthDate != default &&
           UserEducationMajors.Any(x => x.AcademicYear.IsCurrentAcademicYear(DateTime.UtcNow));
  }

  /// <summary>
  ///   Increments the verification attempts counter and updates the last attempt timestamp.
  /// </summary>
  public void IncrementVerificationAttempts()
  {
    VerificationAttempts++;
    LastVerificationAttempt = DateTime.UtcNow;
  }

  /// <summary>
  ///   Resets the verification attempts counter and clears the last attempt timestamp.
  /// </summary>
  public void ResetVerificationAttempts()
  {
    VerificationAttempts = 0;
    LastVerificationAttempt = null;
  }

  /// <summary>
  ///   Increments the verification failures counter and updates the last failure timestamp.
  /// </summary>
  public void IncrementVerificationFailures()
  {
    VerificationFailures++;
    LastVerificationFailure = DateTime.UtcNow;
  }

  /// <summary>
  ///   Resets the verification failures counter and clears the last failure timestamp.
  /// </summary>
  public void ResetVerificationFailures()
  {
    VerificationFailures = 0;
    LastVerificationFailure = null;
  }

  /// <summary>
  ///   Determines if the user is currently blocked from verification attempts.
  /// </summary>
  /// <returns>True if the user is blocked; otherwise, false.</returns>
  public bool IsVerificationBlocked(out TimeSpan remainingBlockTime)
  {
    remainingBlockTime = TimeSpan.Zero;

    if (VerificationFailures >= UserConstants.MaxVerificationAttempts)
    {
      var blockEndTime = LastVerificationFailure?.AddMinutes(UserConstants.VerificationBlockMinutes);
      if (blockEndTime > DateTime.UtcNow)
      {
        remainingBlockTime = blockEndTime.Value - DateTime.UtcNow;
        return true;
      }

      // Reset if block time has passed
      ResetVerificationFailures();
    }

    return false;
  }
}
