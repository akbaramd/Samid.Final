using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Exceptions;
using Samid.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace Samid.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly Random _random = new Random();

        public AuthService(UserManager<User> userManager, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendVerificationCodeAsync(SendCodeRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user == null)
            {
                user = new User { UserName = request.PhoneNumber, PhoneNumber = request.PhoneNumber };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user: {Errors}", createResult.Errors);
                    throw new Exception("Failed to create user.");
                }
            }

            // Check if the user has exceeded the verification attempts limit
            if (user.VerificationAttempts >= 5)
            {
                var blockTime = user.LastVerificationAttempt?.AddMinutes(5);
                if (blockTime > DateTime.UtcNow)
                {
                    var remainingBlockTime = blockTime.Value - DateTime.UtcNow;
                    throw new VerificationFailedException($"Too many attempts. Please try again in {remainingBlockTime.Minutes} minutes and {remainingBlockTime.Seconds} seconds.");
                }
                else
                {
                    user.ResetVerificationAttempts();
                }
            }

            user.IncrementVerificationAttempts();
            await _userManager.UpdateAsync(user);

            var code = _random.Next(100000, 999999).ToString();
            _logger.LogInformation("Generated verification code: {Code}", code);

            // Here, you should send the code via SMS to the user's phone number
            // For demonstration, we'll assume the SMS sending is successful
            await _userManager.SetAuthenticationTokenAsync(user, "Phone", "VerificationCode", code);

            return true;
        }

        public async Task<AuthResponse> VerifyCodeAsync(VerifyCodeRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            // Check if the user has exceeded the verification failure limit
            if (user.VerificationFailures >= 5)
            {
                var blockTime = user.LastVerificationFailure?.AddHours(6);
                if (blockTime > DateTime.UtcNow)
                {
                    var remainingBlockTime = blockTime.Value - DateTime.UtcNow;
                    throw new VerificationFailedException($"Too many verification failures. Please try again in {remainingBlockTime.Hours} hours and {remainingBlockTime.Minutes} minutes.");
                }
                else
                {
                    user.ResetVerificationFailures();
                }
            }

            var code = await _userManager.GetAuthenticationTokenAsync(user, "Phone", "VerificationCode");
            if (code != request.Code)
            {
                user.IncrementVerificationFailures();
                await _userManager.UpdateAsync(user);
                throw new VerificationFailedException("Invalid code.");
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "Phone", "VerificationCode");
            user.ResetVerificationFailures();
            await _userManager.UpdateAsync(user);

            var isProfileComplete = user.IsProfileComplete();
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                IsProfileComplete = isProfileComplete
            };
        }

        public async Task<AuthResponse> CompleteProfileAsync(CompleteProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            user.UpdateName(request.FirstName, request.LastName);
            user.UpdateBirthDate(request.BirthDate);
            await _userManager.UpdateAsync(user);

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                IsProfileComplete = true
            };
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

                o.User.Claims.Add(new("UserName", user.UserName ?? throw new InvalidOperationException()));
                o.User.Claims.Add(("FirstName", user.FirstName ?? string.Empty));
                o.User.Claims.Add(("LastName", user.LastName ?? string.Empty));
                o.User.Claims.Add(("BirthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? "0000-00-00"));
                o.User["UserId"] = user.Id.ToString();
            });

            return jwtToken;
        }
    }
}
