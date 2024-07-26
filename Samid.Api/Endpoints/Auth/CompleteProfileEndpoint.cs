using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Application.DTOs.Authentication;
using Samid.Application.Interfaces;
using System.Security.Claims;

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
        Validator<CompleteProfileRequestValidator>();
    }

    public override async Task HandleAsync(CompleteProfileRequest req, CancellationToken ct)
    {
        // Get the UserId from the authenticated user's claims
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            ThrowError("User ID not found in token.");
            return;
        }

        req.UserId = userId; // Set the UserId in the request

        var response = await _authService.CompleteProfileAsync(req);
        await SendAsync(response, cancellation: ct);
    }
}