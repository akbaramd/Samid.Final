using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs.Authentication;

[Authorize]
public class AuthGetUserProfileEndpoint : EndpointWithoutRequest<AuthUserProfileResponse, AuthGetUserProfileMapper>
{
  private readonly UserManager<User> _userManager;

  public AuthGetUserProfileEndpoint(UserManager<User> userManager)
  {
    _userManager = userManager;
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
        .Include(x=>x.UserAcademicYears)
        .ThenInclude(x=>x.AcademicYear)
        .Include(x=>x.UserAcademicYears)
        .ThenInclude(x=>x.GradeFieldOfStudy).FirstOrDefaultAsync(x =>x.Id == Guid.Parse(userId), cancellationToken: ct);
      if (user == null)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      await SendMappedAsync(user, ct: ct);
    }
  }
}


public class AuthGetUserProfileMapper : ResponseMapper<AuthUserProfileResponse, User>
{
  public override async Task<AuthUserProfileResponse> FromEntityAsync(User user,CancellationToken ct)
  {
    return new AuthUserProfileResponse
    {
      UserId = user.Id,
      UserName = user.UserName,
      PhoneNumber = user.PhoneNumber,
      FirstName = user.FirstName,
      LastName = user.LastName,
      BirthDate = user.BirthDate,
      UserAcademicYears = user.UserAcademicYears.Select(x => new AuthUserProfileResponse.AuthUserProfileAcademicYearsResponse()
      {
        GradeFieldOfStudyId = x.GradeFieldOfStudyId,
        GradeFieldOfStudyTitle = x.GradeFieldOfStudy.Title,
        AcademicYearId = x.AcademicYearId,
        AcademicYearTitle = x.AcademicYear.Title,
        Id = x.Id
      }).ToList()
    };
  }
}
