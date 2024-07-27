using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Samid.Application.DTOs.Authentication;

public class AuthCompleteProfileEndpoint : Endpoint<AuthCompleteProfileRequest, AuthResponse>
{
  private readonly IConfiguration _configuration;
  private readonly UserManager<User> _userManager;

  public AuthCompleteProfileEndpoint(UserManager<User> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  public override void Configure()
  {
    Post("/api/auth/complete-profile");
    Validator<CompleteProfileRequestValidator>();
  }

  public override async Task HandleAsync(AuthCompleteProfileRequest req, CancellationToken ct)
  {
    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      ThrowError("User ID not found in token.");
      return;
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      ThrowError("User not found.");
      return;
    }

    user.UpdateName(req.FirstName, req.LastName);
    user.UpdateBirthDate(req.BirthDate);
    await _userManager.UpdateAsync(user);

    var token = GenerateJwtToken(user);

    var response =
      new AuthResponse { Token = token, Expiration = DateTime.UtcNow.AddHours(1), IsProfileComplete = true };
    await SendAsync(response, cancellation: ct);
  }

  private string GenerateJwtToken(User user)
  {
    var roles = _userManager.GetRolesAsync(user).Result;
    var signingKey = _configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";

    var jwtToken = JwtBearer.CreateToken(o =>
    {
      o.SigningKey = signingKey;
      o.ExpireAt = DateTime.UtcNow.AddDays(1);

      foreach (var role in roles)
      {
        o.User.Roles.Add(role ?? "User");
      }

      o.User.Claims.Add(new Claim(ClaimTypes.NameIdentifier,
        user.Id.ToString() ?? throw new InvalidOperationException()));
      o.User.Claims.Add(new Claim("UserName", user.UserName ?? throw new InvalidOperationException()));
      o.User.Claims.Add(("FirstName", user.FirstName ?? string.Empty));
      o.User.Claims.Add(("LastName", user.LastName ?? string.Empty));
      o.User.Claims.Add(("BirthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? "0000-00-00"));
      o.User["UserId"] = user.Id.ToString();
    });

    return jwtToken;
  }
}
