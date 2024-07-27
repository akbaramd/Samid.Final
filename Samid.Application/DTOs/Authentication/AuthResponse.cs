namespace Samid.Application.DTOs.Authentication;

public class AuthResponse
{
  public string Token { get; set; } = String.Empty;
  public DateTime Expiration { get; set; }
  public bool IsProfileComplete { get; set; }
}
