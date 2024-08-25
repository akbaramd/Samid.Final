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
public class GetMajorsEndpoint(IMapper mapper, ApplicationDbContext context)
  : Endpoint<GetMajorsEndpointFilter, ApiResult<List<EducationMajorsDto>>>
{
  public override void Configure()
  {
    Get(RouteConstants.DefinitionGetEducationMajors);
    Tags("Definitions");
    Summary(c =>
    {
      c.Summary = "Retrieve education majors based on the provided grade ID.";
      c.Description = "This endpoint retrieves a list of education majors filtered by the specified grade ID. If no grade ID is provided, it returns all majors.";
      c.ExampleRequest = new GetMajorsEndpointFilter { GradeId = Guid.Parse("D6F8E38F-3D64-4A16-8FC3-5F3F01AABC45") };

      // Define possible responses and their status codes
      c.Response<ApiResult<List<EducationMajorsDto>>>(200, "Majors retrieved successfully.");
      c.Response<ApiResult<List<EducationMajorsDto>>>(200, "No majors found."); // Success response with empty list
      c.Response<ApiResult>(400, "Invalid request parameters.");
    });
    AllowAnonymous();
    
    Options(x => x.WithTags(RouteConstants.DefinitionPrefix));
  }

  public override async Task HandleAsync(GetMajorsEndpointFilter filter, CancellationToken ct)
  {
    // Query to get the Education Majors based on the filter
    var majors = await context.EducationMajors
      .Include(x => x.EducationBooks)
      .Include(x => x.EducationGrade)
      .Include(x => x.EducationField)
      .Where(x => filter.GradeId == Guid.Empty || x.EducationGradeId == filter.GradeId)
      .ToListAsync(ct);

    if (!majors.Any())
    {
      // Return an empty list with a success message if no majors are found
      await SendAsync(ApiResult<List<EducationMajorsDto>>.Success(new List<EducationMajorsDto>(), "No majors found."),cancellation: ct);
      return;
    }

    // Map the majors to the DTO
    var majorsDto = mapper.Map<List<EducationMajorsDto>>(majors);

    // Return the mapped list wrapped in an ApiResult with a success message
    await SendAsync(ApiResult<List<EducationMajorsDto>>.Success(majorsDto, "Majors retrieved successfully."), cancellation:ct);
  }
}

public class GetMajorsEndpointFilter
{
  [QueryParam, BindFrom("gradeId")]
  public Guid? GradeId { get; set; }
}
