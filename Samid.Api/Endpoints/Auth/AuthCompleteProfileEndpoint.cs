﻿using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.DTOs.Authentication;
using Samid.Domain.Entities;
using Samid.Infrastructure.Persistence;

namespace Samid.Api.Endpoints.Auth;

public class AuthCompleteProfileEndpoint(
  UserManager<User> userManager,
  IConfiguration configuration,
  ApplicationDbContext context)
  : Endpoint<AuthCompleteProfileRequest, AuthResponse>
{
  public override void Configure()
  {
    Post("/api/auth/complete-profile");
    Validator<CompleteProfileRequestValidator>();
  }

  public override async Task HandleAsync(AuthCompleteProfileRequest req, CancellationToken ct)
  {
    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      ThrowError("User ID not found in token.");
      return;
    }

    var user = await userManager.FindByIdAsync(userId);
    if (user == null)
    {
      ThrowError("User not found.");
      return;
    }

    user.UpdateName(req.FirstName, req.LastName);
    user.UpdateBirthDate(req.BirthDate);
    await userManager.UpdateAsync(user);

    var gradeFieldOfEducation = await context.EducationMajors
      .Include(gfs => gfs.EducationGrade)
      .Include(gfs => gfs.EducationField)
      .FirstOrDefaultAsync(gfs => gfs.EducationGradeId == req.GradeOfEducationId &&
                                  gfs.EducationFieldId == req.FieldOfEducationId, ct);

    if (gradeFieldOfEducation == null)
    {
      ThrowError("GradeFieldOfEducation not found.");
      return;
    }

    var academicYear = await context.AcademicYears
      .FirstOrDefaultAsync(ay => ay.StartDate <= DateTime.UtcNow && ay.EndDate >= DateTime.Now, ct);

    if (academicYear == null)
    {
      academicYear = new AcademicYear();
      context.AcademicYears.Add(academicYear);
      await context.SaveChangesAsync(ct);
    }

    var userAcademicYearExists = await context.UserEducationMajors
      .AnyAsync(ay => ay.UserId == user.Id && ay.EducationMajorsId == gradeFieldOfEducation.Id, ct);

    if (!userAcademicYearExists)
    {
      var userAcademicYear = new UserEducationMajors(user.Id, gradeFieldOfEducation.Id, academicYear.Id);
      context.UserEducationMajors.Add(userAcademicYear);
      await context.SaveChangesAsync(ct);
    }

    var token = GenerateJwtToken(user);

    var response = new AuthResponse
    {
      Token = token, Expiration = DateTime.UtcNow.AddHours(1), IsProfileComplete = true
    };
    await SendAsync(response, cancellation: ct);
  }

  private string GenerateJwtToken(User user)
  {
    var roles = userManager.GetRolesAsync(user).Result;
    var signingKey = configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";

    var jwtToken = JwtBearer.CreateToken(o =>
    {
      o.SigningKey = signingKey;
      o.ExpireAt = DateTime.UtcNow.AddDays(1);

      foreach (var role in roles)
      {
        o.User.Roles.Add(role ?? "User");
      }

      o.User.Claims.Add(new Claim(ClaimTypes.NameIdentifier,
        user.Id.ToString() ?? throw new InvalidOperationException()));
      o.User.Claims.Add(new Claim("UserName", user.UserName ?? throw new InvalidOperationException()));
      o.User.Claims.Add(("FirstName", user.FirstName ?? string.Empty));
      o.User.Claims.Add(("LastName", user.LastName ?? string.Empty));
      o.User.Claims.Add(("BirthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? "0000-00-00"));
      o.User["UserId"] = user.Id.ToString();
    });

    return jwtToken;
  }
}
