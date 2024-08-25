using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Results;
using Samid.Application.DTOs.Activity;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Samid.Api.Endpoints.Activity;

public class StudyActivityRegisterEndpoint(
    ApplicationDbContext context,
    ILogger<StudyActivityRegisterEndpoint> logger)
    : Endpoint<StudyActivityRequest, ApiResult>
{
  private readonly ILogger<StudyActivityRegisterEndpoint> _logger = logger;

  public override void Configure()
    {
        Post(RouteConstants.ActivityStudyRegister);
        Validator<StudyActivityRequestValidator>();
        
        Summary(summary =>
        {
          summary.Description = "Registers a new study activity for a specific educational major and book. This endpoint allows users to log study time, associated with a particular book and major, providing a description of the activity.";

          summary.ExampleRequest = new StudyActivityRequest
          {
            UserEducationalMajorId = Guid.Parse("CE093FF8-94D8-4CB2-BD2D-6236AE006D7D"),
            EducationalBookId = Guid.Parse("2d06f1e9-a479-4f9f-a3be-538c27c0a228"),
            Duration = TimeSpan.FromMinutes(45),
            Description = "Studied Chapter 4 of the math book."
          };

          summary.Responses[200] = "The study activity was successfully registered.";
          summary.Response<ApiResult>(200, "OK - The study activity was successfully registered.", example: new ApiResult(200, "The study activity was successfully registered."));

          summary.Responses[400] = "Bad Request - The request is invalid. Ensure all required fields are provided.";
          summary.Response<ApiResult>(400, "Bad Request - User ID is missing or invalid, or the request contains invalid data.", example: ApiResult.Error("User ID is missing or invalid.", StatusCodes.Status400BadRequest));

          summary.Responses[404] = "Not Found - The user was not found.";
          summary.Response<ApiResult>(404, "Not Found - Failed to find user.", example: ApiResult.Error("Failed to find user.", StatusCodes.Status404NotFound));
        });

        Options(options => options.WithTags(RouteConstants.ActivityPrefix));
    }

    public override async Task HandleAsync(StudyActivityRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            await SendAsync(ApiResult.Error("User ID is missing or invalid.", StatusCodes.Status400BadRequest),cancellation: ct);
            return;
        }

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userGuid, ct);
        if (user == null)
        {
            await SendAsync(ApiResult.Error("Failed to find user.", StatusCodes.Status404NotFound), cancellation:ct);
            return;
        }

        var studyActivity = new StudyActivity(userGuid, req.UserEducationalMajorId, req.EducationalBookId,
            req.Duration, req.Description);

        await context.StudyActivities.AddAsync(studyActivity, ct);
        await context.SaveChangesAsync(ct);

        await SendAsync(ApiResult.Success("The study activity was successfully registered.", StatusCodes.Status200OK),cancellation: ct);
    }
}
