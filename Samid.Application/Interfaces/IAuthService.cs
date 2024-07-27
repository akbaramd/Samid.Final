using Samid.Application.DTOs.Authentication;

namespace Samid.Application.Interfaces;

public interface IAuthService
{
  Task<bool> SendVerificationCodeAsync(AuthSendCodeRequest request);
  Task<AuthResponse> VerifyCodeAsync(AuthVerifyCodeRequest request);
  Task<AuthResponse> CompleteProfileAsync(AuthCompleteProfileRequest request);
  Task<AuthUserProfileResponse> GetUserProfileAsync(string userId);
}
