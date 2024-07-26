using Samid.Application.DTOs.Authentication;

namespace Samid.Application.Interfaces;

public interface IAuthService
{
  Task<bool> SendVerificationCodeAsync(SendCodeRequest request);
  Task<AuthResponse> VerifyCodeAsync(VerifyCodeRequest request);
  Task<AuthResponse> CompleteProfileAsync(CompleteProfileRequest request);
}