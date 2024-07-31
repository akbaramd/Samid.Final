using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Definitions;

[Authorize]
public class GetMajorsEndpoint(IMapper mapper, ApplicationDbContext context)
  : Endpoint<GetMajorsEndpointFilter, List<EducationMajorsDto>>
{
  public override void Configure()
  {
    Get("/api/definitions/majors");
    Tags("Definitions");
    AllowAnonymous();
    Options(x=>x.WithTags("Definitions"));
  }

  public override async Task HandleAsync(GetMajorsEndpointFilter  filter,CancellationToken ct)
  {
    var stages = context.EducationMajors.Include(x=>x.EducationBooks)
      .Include(x=>x.EducationGrade)
      .Include(x=>x.EducationField).Where(x => filter.GradeId == Guid.Empty || x.EducationGradeId == filter.GradeId);

    await SendAsync(mapper.Map<List<EducationMajors>, List<EducationMajorsDto>>(stages.ToList()), cancellation: ct);
  }
}

public class GetMajorsEndpointFilter
{
  [QueryParam,BindFrom("gradeId")]
  public Guid? GradeId { get; set; }
}