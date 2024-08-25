using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Samid.Api.Results;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Samid.Api.Endpoints.Auth;

public class AuthSendVerificationCodeEndpoint(
    UserManager<User> userManager,
    ILogger<AuthSendVerificationCodeEndpoint> logger)
    : Endpoint<AuthSendCodeRequest, ApiResult>
{
    private readonly Random _random = new();

    public override void Configure()
    {
        Post(RouteConstants.AuthSendCode);
        AllowAnonymous();
        Validator<SendCodeRequestValidator>();
        Summary(c =>
        {
          c.Summary = "Send a verification code to the user's phone number.";
          c.Description = "This endpoint sends a verification code via SMS to the user's phone number for authentication purposes.";
          c.ExampleRequest = new AuthSendCodeRequest { PhoneNumber = "09371770774" };

          // Define possible responses and their status codes
          c.Response<ApiResult>(200, "Verification code sent successfully.");
          c.Response<ApiResult>(400, "Too many attempts. Please try again later.");
          c.Response<ApiResult>(500, "Failed to create user.");
        });

        Options(x => x.WithTags(RouteConstants.AuthPrefix));
    }

    public override async Task HandleAsync(AuthSendCodeRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(req.PhoneNumber);
        if (user == null)
        {
            user = new User { UserName = req.PhoneNumber, PhoneNumber = req.PhoneNumber };
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create user: {Errors}", createResult.Errors);
                await SendAsync(ApiResult.Error("Failed to create user.", StatusCodes.Status500InternalServerError), StatusCodes.Status500InternalServerError, ct);
                return;
            }
        }

        // Check if the user is currently blocked from verification attempts
        if (user.IsVerificationBlocked(out var remainingBlockTime))
        {
            var message = "Too many attempts. Please try again later.";
            var blockTimeInMilliseconds = remainingBlockTime.TotalMilliseconds;

            var result = ApiResult.BadRequest(message, new { RemainingBlockTimeInMs = blockTimeInMilliseconds });
            await SendAsync(result, StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Increment verification attempts and update the user
        user.IncrementVerificationAttempts();
        await userManager.UpdateAsync(user);

        // Generate a verification code (replace with a random code generation logic in production)
        var code = "12345";
        // var code = _random.Next(10000, 99999).ToString();
        logger.LogInformation("Generated verification code: {Code}", code);

        // Here, you should send the code via SMS to the user's phone number
        // For demonstration, we'll assume the SMS sending is successful
        await userManager.SetAuthenticationTokenAsync(user, "Phone", "VerificationCode", code);

        await SendAsync(ApiResult.Success("Verification code sent successfully."), StatusCodes.Status200OK, ct);
    }
}
