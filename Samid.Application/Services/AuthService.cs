using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints.Security;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Interfaces;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly Random _random = new Random();

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<bool> SendVerificationCodeAsync(SendCodeRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        
        
        if (user == null)
        {
            user = new User { UserName = request.PhoneNumber, PhoneNumber = request.PhoneNumber };
            await _userManager.CreateAsync(user);
        }

        var code = _random.Next(100000, 999999).ToString();
        Console.WriteLine(code);
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
          throw new UnauthorizedAccessException("Invalid user.");
        }
        
        
       
          var code = await _userManager.GetAuthenticationTokenAsync(user, "Phone", "VerificationCode");
         
          if (user == null || code != request.Code)
          {
            throw new UnauthorizedAccessException("Invalid code.");
          }
       

        await _userManager.RemoveAuthenticationTokenAsync(user, "Phone", "VerificationCode");

        var isProfileComplete = user.IsProfileComplete();

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            IsProfileComplete = isProfileComplete
        };
    }

    private string GenerateJwtToken(User user)
    {
      // Extract roles from the user
      var roles = _userManager.GetRolesAsync(user).Result;

      var jwtToken = JwtBearer.CreateToken(
        o =>
        {
          o.SigningKey = _configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";  // Use the configured signing key
          o.ExpireAt = DateTime.UtcNow.AddDays(1);   // Set token expiration

          // Add roles to the token
          foreach (var role in roles)
          {
            o.User.Roles.Add(role ?? "User");  // Fallback role to "User" if role is null
          }

          // Add user claims to the token
          o.User.Claims.Add(new ("UserName", user.UserName ?? throw new InvalidOperationException()));
          o.User.Claims.Add(("FirstName", user.FirstName ?? string.Empty));
          o.User.Claims.Add(("LastName", user.LastName ?? string.Empty));
          o.User.Claims.Add(("BirthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? "0000-00-00"));
          o.User["UserId"] = user.Id.ToString();  // Indexer based claim setting
        });

      return jwtToken;
    }



    public async Task<AuthResponse> CompleteProfileAsync(CompleteProfileRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid user.");
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

    
}
