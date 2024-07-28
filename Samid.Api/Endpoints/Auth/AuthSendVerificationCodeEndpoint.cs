using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;

namespace Samid.Api.Endpoints.Auth;

public class AuthSendVerificationCodeEndpoint : Endpoint<AuthSendCodeRequest>
{
  private readonly ILogger<AuthSendVerificationCodeEndpoint> _logger;
  private readonly Random _random = new();
  private readonly UserManager<User> _userManager;

  public AuthSendVerificationCodeEndpoint(UserManager<User> userManager,
    ILogger<AuthSendVerificationCodeEndpoint> logger)
  {
    _userManager = userManager;
    _logger = logger;
  }

  public override void Configure()
  {
    Post("/api/auth/send-code");
    AllowAnonymous();
    Validator<SendCodeRequestValidator>();
    Summary(c => c.ExampleRequest = new AuthSendCodeRequest { PhoneNumber = "09371770774" });
  }

  public override async Task HandleAsync(AuthSendCodeRequest req, CancellationToken ct)
  {
    var user = await _userManager.FindByNameAsync(req.PhoneNumber);
    if (user == null)
    {
      user = new User { UserName = req.PhoneNumber, PhoneNumber = req.PhoneNumber };
      var createResult = await _userManager.CreateAsync(user);
      if (!createResult.Succeeded)
      {
        _logger.LogError("Failed to create user: {Errors}", createResult.Errors);
        ThrowError("Failed to create user.");
        return;
      }
    }

    // Check if the user has exceeded the verification attempts limit
    if (user.VerificationAttempts >= 5)
    {
      var blockTime = user.LastVerificationAttempt?.AddMinutes(5);
      if (blockTime > DateTime.UtcNow)
      {
        var remainingBlockTime = blockTime.Value - DateTime.UtcNow;
        ThrowError(
          $"Too many attempts. Please try again in {remainingBlockTime.Minutes} minutes and {remainingBlockTime.Seconds} seconds.");
        return;
      }

      user.ResetVerificationAttempts();
    }

    user.IncrementVerificationAttempts();
    await _userManager.UpdateAsync(user);

    // var code = _random.Next(100000, 999999).ToString();
    var code = "123456";
    _logger.LogInformation("Generated verification code: {Code}", code);

    // Here, you should send the code via SMS to the user's phone number
    // For demonstration, we'll assume the SMS sending is successful
    await _userManager.SetAuthenticationTokenAsync(user, "Phone", "VerificationCode", code);

    await SendOkAsync(ct);
  }
}
