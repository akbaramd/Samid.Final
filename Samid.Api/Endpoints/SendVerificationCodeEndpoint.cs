using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Interfaces;

public class SendVerificationCodeEndpoint : Endpoint<SendCodeRequest>
{
  private readonly IAuthService _authService;

  public SendVerificationCodeEndpoint(IAuthService authService)
  {
    _authService = authService;
  }

  public override void Configure()
  {
    Post("/api/auth/send-code");
    AllowAnonymous();
    Validator<SendCodeRequestValidator>();
  }

  public override async Task HandleAsync(SendCodeRequest req, CancellationToken ct)
  {
    var result = await _authService.SendVerificationCodeAsync(req);
    if (result)
    {
      await SendOkAsync(ct);
    }
    else
    {
      await SendUnauthorizedAsync(ct);
    }
  }
}
