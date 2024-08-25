using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Api.Results;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Definitions;

[Authorize]
public class GetGradesEndpoint(IMapper mapper, ApplicationDbContext context)
  : Endpoint<GetGradesEndpointFilter, ApiResult<List<EducationGradeDto>>>
{
  public override void Configure()
  {
    Get(RouteConstants.DefinitionGetEducationGrades);
    Tags("Definitions");
    AllowAnonymous();
    Summary(c =>
    {
      c.Summary = "Retrieve education grades based on the provided stage ID.";
      c.Description = "This endpoint retrieves a list of education grades filtered by the specified stage ID. If no stage ID is provided, it returns all grades.";
      c.ExampleRequest = new GetGradesEndpointFilter { StageId = Guid.Parse("D6F8E38F-3D64-4A16-8FC3-5F3F01AABC45") };

      // Define possible responses and their status codes
      c.Response<ApiResult<List<EducationGradeDto>>>(200, "Grades retrieved successfully.");
      c.Response<ApiResult<List<EducationGradeDto>>>(200, "No grades found."); // Success response with empty list
      c.Response<ApiResult>(400, "Invalid request parameters.");
    });

    Options(x => x.WithTags(RouteConstants.DefinitionPrefix));
  }

  public override async Task HandleAsync(GetGradesEndpointFilter filter, CancellationToken ct)
  {
    var stages = context.EducationGrades
      .Where(x => filter.StageId == Guid.Empty || x.EducationStageId == filter.StageId)
      .ToList();

    if (!stages.Any())
    {
      // If no grades are found, return an empty list with a success message
      await SendAsync(ApiResult<List<EducationGradeDto>>.Success(new List<EducationGradeDto>(), "No grades found."), cancellation:ct);
      return;
    }

    // Map the list of stages to the DTO
    var gradeDtos = mapper.Map<List<EducationGrade>, List<EducationGradeDto>>(stages);

    // Return the mapped list wrapped in an ApiResult with a success message
    await SendAsync(ApiResult<List<EducationGradeDto>>.Success(gradeDtos, "Grades retrieved successfully."), cancellation:ct);
  }
}

public class GetGradesEndpointFilter
{
  [QueryParam, BindFrom("stageId")]
  public Guid? StageId { get; set; }
}
