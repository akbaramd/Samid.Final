using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Results;
using Samid.Application.DTOs;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Auth;

[Authorize]
public class AuthGetUserProfileEndpoint(UserManager<User> userManager, IMapper mapper)
  : EndpointWithoutRequest<ApiResult<AuthUserProfileResponse>>
{
  public override void Configure()
  {
    Get(RouteConstants.AuthGetProfile);
    Summary(c =>
    {
      c.Summary = "Retrieve the authenticated user's profile.";
      c.Description = "This endpoint retrieves the profile information of the currently authenticated user, including their education majors and related details.";
    
      c.Response<ApiResult<AuthUserProfileResponse>>(200, "User profile retrieved successfully.");
      c.Response<ApiResult<AuthUserProfileResponse>>(401, "User ID not found in token.");
      c.Response<ApiResult<AuthUserProfileResponse>>(404, "User not found.");
    });

    Options(x => x.WithTags(RouteConstants.AuthPrefix));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    // Retrieve the user's ID from the JWT claims
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
    {
      // Return an unauthorized response using ApiResult
      await SendAsync(ApiResult<AuthUserProfileResponse>.Unauthorized("User ID not found in token."), cancellation: ct);
      return;
    }

    // Find the user in the database with the associated ID
    var user = await userManager.Users
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
      // If no user is found with the given ID, return a 404 Not Found response
      await SendAsync(ApiResult<AuthUserProfileResponse>.NotFound("User not found."), cancellation: ct);
      return;
    }

    // Map the user entity to a DTO and send the response wrapped in ApiResult
    var userDto = mapper.Map<UserDto>(user);
    var response = new AuthUserProfileResponse { User = userDto };

    await SendAsync(ApiResult<AuthUserProfileResponse>.Success(response, "User profile retrieved successfully."),
      cancellation: ct);
  }
}
