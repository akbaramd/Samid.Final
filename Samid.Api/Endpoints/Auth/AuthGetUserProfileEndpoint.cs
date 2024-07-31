﻿using System.Security.Claims;
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
public class AuthGetUserProfileEndpoint(UserManager<User> userManager, IMapper mapper)
  : EndpointWithoutRequest<AuthUserProfileResponse>
{
  public override void Configure()
  {
    Get("/api/auth/profile");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId != null)
    {
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
        await SendNotFoundAsync(ct);
        return;
      }

      
      await SendAsync(new AuthUserProfileResponse { User = mapper.Map<UserDto>(user) }, cancellation: ct);
    }
  }
}
