using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Endpoints.Auth;
using Samid.Application.DTOs.Activity;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;

namespace Samid.Api.Endpoints.Activity;

public class StudyActivityRegisterEndpoint(
  ApplicationDbContext userManager,
  ILogger<AuthSendVerificationCodeEndpoint> logger)
  : Endpoint<StudyActivityRequest>
{
  private readonly ILogger<AuthSendVerificationCodeEndpoint> _logger = logger;
  private readonly Random _random = new();

  public override void Configure()
  {
    Post("/api/activity/study");  
    Validator<StudyActivityRequestValidator>();
    Summary(c => c.ExampleRequest = new StudyActivityRequest
    {
      EducationalMajorId = Guid.Parse("ce093ff8-94d8-4cb2-bd2d-6236ae006d7d"),
      EducationalBookId = Guid.Parse("2d06f1e9-a479-4f9f-a3be-538c27c0a228"),
      Duration = TimeSpan.FromMinutes(45),
      Description = "test activity"
    });
  }

  public override async Task HandleAsync(StudyActivityRequest req, CancellationToken ct)
  {
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??"");
    var user = await userManager.Users.FirstOrDefaultAsync(x=>x.Id == userId, cancellationToken: ct);
    if (user == null)
    {
        ThrowError("Failed to find user.");
    }

    await userManager.StudyActivities.AddAsync(new StudyActivity(userId, req.EducationalMajorId, req.EducationalBookId,
      req.Duration, req.Description), ct);

    await userManager.SaveChangesAsync(ct);
    
    
    await SendOkAsync(ct);
  }
}
