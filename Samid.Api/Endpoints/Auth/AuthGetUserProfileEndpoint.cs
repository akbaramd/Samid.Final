using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Auth;

[Authorize]
public class AuthGetUserProfileEndpoint : EndpointWithoutRequest<AuthUserProfileResponse>
{
  private readonly IMapper _mapper;
  private readonly UserManager<User> _userManager;

  public AuthGetUserProfileEndpoint(UserManager<User> userManager, IMapper mapper)
  {
    _userManager = userManager;
    _mapper = mapper;
  }

  public override void Configure()
  {
    Get("/api/auth/profile");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId != null)
    {
      var user = await _userManager.Users
        .Include(x => x.UserAcademicYears)
        .ThenInclude(x => x.AcademicYear)
        .Include(x => x.UserAcademicYears)
        .ThenInclude(x => x.GradeFieldOfStudy)
        .ThenInclude(x => x.FieldOfStudy)
        .Include(x => x.UserAcademicYears)
        .ThenInclude(x => x.GradeFieldOfStudy)
        .ThenInclude(x => x.GradeOfStudy)
        .FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId), ct);
      if (user == null)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      await SendAsync(new AuthUserProfileResponse { User = _mapper.Map<UserDto>(user) }, cancellation: ct);
    }
  }
}
