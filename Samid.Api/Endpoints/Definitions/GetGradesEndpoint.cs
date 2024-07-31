using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Definitions;

[Authorize]
public class GetGradesEndpoint(IMapper mapper, ApplicationDbContext context)
  : Endpoint<GetGradesEndpointFilter, List<EducationGradeDto>>
{
  public override void Configure()
  {
    Get("/api/definitions/grades");
    Tags("Definitions");
    AllowAnonymous();
    Options(x=>x.WithTags("Definitions"));
  }

  public override async Task HandleAsync(GetGradesEndpointFilter filter,CancellationToken ct)
  {
    var stages = context.EducationGrades.Where(x => filter.StageId == Guid.Empty || x.EducationStageId == filter.StageId);
    await SendAsync(mapper.Map<List<EducationGrade>, List<EducationGradeDto>>(stages.ToList()), cancellation: ct);
  }
}

public class GetGradesEndpointFilter
{
  [QueryParam,BindFrom("stageId")]
  public Guid? StageId { get; set; }
}