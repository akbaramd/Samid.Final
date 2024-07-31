using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Definitions;

[Authorize]
public class GetStagesEndpoint(IMapper mapper, ApplicationDbContext context)
  : EndpointWithoutRequest<List<EducationStageDto>>
{
  public override void Configure()
  {
    Get("/api/definitions/stages");
    Tags("Definitions");
    AllowAnonymous();
    Options(x => x.WithTags("Definitions"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var stages = context.EducationStages.ToList();
    await SendAsync(mapper.Map<List<EducationStage>, List<EducationStageDto>>(stages), cancellation: ct);
  }
}
