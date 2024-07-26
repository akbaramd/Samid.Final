using Microsoft.AspNetCore.Identity;
using System;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public int VerificationAttempts { get; private set; }
    public DateTime? LastVerificationAttempt { get; private set; }
    public int VerificationFailures { get; private set; }
    public DateTime? LastVerificationFailure { get; private set; }

    // Parameterless constructor for EF Core
    public User() { }

    // Public constructor for use in application code
    public User(string firstName, string lastName, DateTime birthDate)
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
