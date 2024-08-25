using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Results;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;

namespace Samid.Api.Endpoints.Auth;

public class AuthVerifyCodeEndpoint(UserManager<User> userManager, IConfiguration configuration)
    : Endpoint<AuthVerifyCodeRequest, ApiResult<AuthResponse>>
{
    public override void Configure()
    {
        Post(RouteConstants.AuthVerifyCode);
        AllowAnonymous();
        Validator<VerifyCodeRequestValidator>();
        Summary(c =>
        {
          c.ExampleRequest = new AuthVerifyCodeRequest { PhoneNumber = "09371770774", Code = "12345" };
          c.Response<ApiResult<AuthResponse>>(200, "Successful response with AuthResponse");
          c.Response<ApiResult>(400, "Bad request response");
          c.Response<ApiResult>(404, "Not found response");
          c.Response<ApiResult>(429, "Too many requests");
        });
        Options(x => x.WithTags(RouteConstants.AuthPrefix));
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
            await SendAsync(ApiResult.NotFound("User not found."), cancellation: ct);
            return;
        }

        // Check if the user is currently blocked from verification attempts
        if (user.IsVerificationBlocked(out var remainingBlockTime))
        {
            var blockTimeInMilliseconds = remainingBlockTime.TotalMilliseconds;
            var message = $"Too many verification failures. Please try again in {remainingBlockTime.Minutes} minutes and {remainingBlockTime.Seconds} seconds.";
            var result = ApiResult.Error(message, StatusCodes.Status429TooManyRequests, new { RemainingBlockTimeInMs = blockTimeInMilliseconds });
            await SendAsync(result, StatusCodes.Status429TooManyRequests, ct);
            return;
        }

        var code = await userManager.GetAuthenticationTokenAsync(user, "Phone", "VerificationCode");
        if (code != req.Code)
        {
            user.IncrementVerificationFailures();
            await userManager.UpdateAsync(user);
            await SendAsync(ApiResult.Error("Invalid code."), StatusCodes.Status400BadRequest, cancellation: ct);
            return;
        }

        await userManager.RemoveAuthenticationTokenAsync(user, "Phone", "VerificationCode");
        user.ResetVerificationFailures();
        await userManager.UpdateAsync(user);

        var isProfileComplete = user.IsProfileComplete();
        var token = GenerateJwtToken(user);

        var response = new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            IsProfileComplete = isProfileComplete
        };

        await SendAsync(ApiResult<AuthResponse>.Success(response, "Verification successful."), cancellation: ct);
    }

    private string GenerateJwtToken(User user)
    {
        var roles = userManager.GetRolesAsync(user).Result;
        var signingKey = configuration["Jwt:Key"] ?? ApplicationConstants.DefaultJwtSigningKey;

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
