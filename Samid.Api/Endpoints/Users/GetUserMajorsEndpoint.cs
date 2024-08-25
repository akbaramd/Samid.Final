using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Api.Results;
using Samid.Application.DTOs;
using Samid.Domain.Entities;
using IMapper = AutoMapper.IMapper;

namespace Samid.Api.Endpoints.Users;

[Authorize]
public class GetUserMajorsEndpoint : EndpointWithoutRequest<ApiResult<List<UserEducationMajorsDto>>>
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
        Get(RouteConstants.UserGetEducationMajors);
        Summary(c =>
        {
          c.Summary = "Retrieve the education majors associated with the authenticated user.";
          c.Description = "This endpoint retrieves all education majors linked to the authenticated user, including details about the academic year, education field, grade, and books.";
    
          // Define possible responses and their status codes
          c.Response<ApiResult<List<UserEducationMajorsDto>>>(200, "User majors retrieved successfully.");
          c.Response<ApiResult<List<UserEducationMajorsDto>>>(401, "User ID not found in token."); // Unauthorized response if user ID is missing
          c.Response<ApiResult<List<UserEducationMajorsDto>>>(404, "User not found."); // Not found response if the user doesn't exist
          c.Response<ApiResult>(500, "An internal server error occurred while retrieving the user majors.");
        });

        Options(x => x.WithTags(RouteConstants.UserPrefix));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Retrieve the user's ID from the JWT claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            // If the user ID is not found, return an unauthorized response
            await SendAsync(ApiResult<List<UserEducationMajorsDto>>.Unauthorized("User ID not found in token."), cancellation:ct);
            return;
        }

        // Find the user in the database with the associated ID
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
            // If no user is found with the given ID, return a 404 Not Found response
            await SendAsync(ApiResult<List<UserEducationMajorsDto>>.NotFound("User not found."), cancellation:ct);
            return;
        }

        // Map the user's education majors to DTOs
        var userMajorsDto = _mapper.Map<List<UserEducationMajorsDto>>(user.UserEducationMajors);

        // Return the mapped list wrapped in an ApiResult with a success message
        await SendAsync(ApiResult<List<UserEducationMajorsDto>>.Success(userMajorsDto, "User majors retrieved successfully."), cancellation:ct);
    }
}
