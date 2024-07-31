using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Users;

[Authorize]
public class GetUserMajorsEndpoint : EndpointWithoutRequest<List<UserEducationMajorsDto>>
{
  private readonly IMapper _mapper;
  private readonly UserManager<User> _userManager;

  public GetUserMajorsEndpoint(UserManager<User> userManager, IMapper mapper)
  {
    _userManager = userManager;
    _mapper = mapper;
  }

  public override void Configure()
  {
    Get("/api/user/education-majors");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId != null)
    {
      var user = await _userManager.Users
        .Include(x => x.UserEducationMajors)
        .ThenInclude(x => x.AcademicYear)
        .Include(x => x.UserEducationMajors)
        .ThenInclude(x => x.EducationMajors)
        .ThenInclude(x => x.EducationField)
        .Include(x => x.UserEducationMajors)
        .ThenInclude(x => x.EducationMajors)
        .ThenInclude(x => x.EducationGrade)
        .Include(x => x.UserEducationMajors)
        .ThenInclude(x => x.EducationMajors)
        .ThenInclude(x => x.EducationBooks)
        .FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId), ct);
      if (user == null)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      
      await SendAsync( _mapper.Map<List<UserEducationMajorsDto>>(user.UserEducationMajors) , cancellation: ct);
    }
  }
}
