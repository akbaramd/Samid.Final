using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Results;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Definitions;

[Authorize]
public class GetStagesEndpoint(IMapper mapper, ApplicationDbContext context)
  : EndpointWithoutRequest<ApiResult<List<EducationStageDto>>>
{
  public override void Configure()
  {
    Get(RouteConstants.DefinitionGetEducationStages);
    Tags("Definitions");
    AllowAnonymous();
    Summary(c =>
    {
      c.Summary = "Retrieve all education stages.";
      c.Description = "This endpoint retrieves a list of all available education stages.";
    
      // Define possible responses and their status codes
      c.Response<ApiResult<List<EducationStageDto>>>(200, "Stages retrieved successfully.");
      c.Response<ApiResult<List<EducationStageDto>>>(200, "No stages found."); // Success response with an empty list if no stages are available
      c.Response<ApiResult>(500, "An internal server error occurred while retrieving the stages.");
    });
    Options(x => x.WithTags(RouteConstants.DefinitionPrefix));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    // Fetch all education stages from the database
    var stages = await context.EducationStages.ToListAsync(ct);

    if (!stages.Any())
    {
      // Return an empty list with a success message if no stages are found
      await SendAsync(ApiResult<List<EducationStageDto>>.Success(new List<EducationStageDto>(), "No stages found."),cancellation: ct);
      return;
    }

    // Map the stages to the DTO
    var stageDtos = mapper.Map<List<EducationStageDto>>(stages);

    // Return the mapped list wrapped in an ApiResult with a success message
    await SendAsync(ApiResult<List<EducationStageDto>>.Success(stageDtos, "Stages retrieved successfully."),cancellation: ct);
  }
}
