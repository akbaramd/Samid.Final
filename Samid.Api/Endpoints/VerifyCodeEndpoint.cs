using FastEndpoints;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Interfaces;

namespace Samid.Api.Endpoints;

public class VerifyCodeEndpoint : Endpoint<VerifyCodeRequest, AuthResponse>
{
  private readonly IAuthService _authService;

  public VerifyCodeEndpoint(IAuthService authService)
  {
    _authService = authService;
  }

  public override void Configure()
  {
    Post("/api/auth/verify-code");
    AllowAnonymous();
    Validator<VerifyCodeRequestValidator>();
  }

  public override async Task HandleAsync(VerifyCodeRequest req, CancellationToken ct)
  {
    var response = await _authService.VerifyCodeAsync(req);
    await SendAsync(response, cancellation: ct);
  }
}