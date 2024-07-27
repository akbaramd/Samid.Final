﻿namespace Samid.Application.DTOs.Authentication;

public class AuthUserProfileResponse
{
  public Guid UserId { get; set; }
  public string? UserName { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; } = string.Empty;
  public string? FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; } = string.Empty;
  public DateTime? BirthDate { get; set; }
}
