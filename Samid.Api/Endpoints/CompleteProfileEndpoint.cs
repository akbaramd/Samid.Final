using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Interfaces;

public class CompleteProfileEndpoint : Endpoint<CompleteProfileRequest, AuthResponse>
{
  private readonly IAuthService _authService;

  public CompleteProfileEndpoint(IAuthService authService)
  {
    _authService = authService;
  }

  public override void Configure()
  {
    Post("/api/auth/complete-profile");
    AllowAnonymous();
    Validator<CompleteProfileRequestValidator>();
  }

  public override async Task HandleAsync(CompleteProfileRequest req, CancellationToken ct)
  {
    var response = await _authService.CompleteProfileAsync(req);
    await SendAsync(response, cancellation: ct);
  }
}
