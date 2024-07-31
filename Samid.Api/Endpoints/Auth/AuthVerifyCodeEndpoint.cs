using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;

namespace Samid.Api.Endpoints.Auth;

public class AuthVerifyCodeEndpoint(UserManager<User> userManager, IConfiguration configuration)
  : Endpoint<AuthVerifyCodeRequest, AuthResponse>
{
  public override void Configure()
  {
    Post("/api/auth/verify-code");
    AllowAnonymous();
    Validator<VerifyCodeRequestValidator>();
    Summary(c => c.ExampleRequest = new AuthVerifyCodeRequest { PhoneNumber = "09371770774", Code = "123456" });
  }

  public override async Task HandleAsync(AuthVerifyCodeRequest req, CancellationToken ct)
  {
    var user = await userManager.Users
      .Include(x => x.UserEducationMajors)
      .ThenInclude(x => x.AcademicYear)
      .Include(x => x.UserEducationMajors)
      .ThenInclude(x => x.EducationMajors)
      .ThenInclude(x => x.EducationField)
      .Include(x => x.UserEducationMajors)
      .ThenInclude(x => x.EducationMajors)
      .ThenInclude(x => x.EducationGrade)
      .FirstOrDefaultAsync(x => x.UserName != null && x.UserName.Equals(req.PhoneNumber), ct);

    if (user == null)
    {
      ThrowError("User not found.");
      return;
    }

    // Check if the user has exceeded the verification failure limit
    if (user.VerificationFailures >= 5)
    {
      var blockTime = user.LastVerificationFailure?.AddHours(6);
      if (blockTime > DateTime.UtcNow)
      {
        var remainingBlockTime = blockTime.Value - DateTime.UtcNow;
        ThrowError(
          $"Too many verification failures. Please try again in {remainingBlockTime.Hours} hours and {remainingBlockTime.Minutes} minutes.");
        return;
      }

      user.ResetVerificationFailures();
    }

    var code = await userManager.GetAuthenticationTokenAsync(user, "Phone", "VerificationCode");
    if (code != req.Code)
    {
      user.IncrementVerificationFailures();
      await userManager.UpdateAsync(user);
      ThrowError("Invalid code.");
      return;
    }

    await userManager.RemoveAuthenticationTokenAsync(user, "Phone", "VerificationCode");
    user.ResetVerificationFailures();
    await userManager.UpdateAsync(user);

    var isProfileComplete = user.IsProfileComplete();
    var token = GenerateJwtToken(user);

    var response = new AuthResponse
    {
      Token = token, Expiration = DateTime.UtcNow.AddHours(1), IsProfileComplete = isProfileComplete
    };

    await SendAsync(response, cancellation: ct);
  }

  private string GenerateJwtToken(User user)
  {
    var roles = userManager.GetRolesAsync(user).Result;
    var signingKey = configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";

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
