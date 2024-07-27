using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Samid.Application.DTOs.Authentication;

[Authorize]
public class AuthGetUserProfileEndpoint : EndpointWithoutRequest<AuthUserProfileResponse, AuthGetUserProfileMapper>
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
      var user = await _userManager.FindByIdAsync(userId);
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
  public override AuthUserProfileResponse FromEntity(User user)
  {
    return new AuthUserProfileResponse
    {
      UserId = user.Id,
      UserName = user.UserName,
      PhoneNumber = user.PhoneNumber,
      FirstName = user.FirstName,
      LastName = user.LastName,
      BirthDate = user.BirthDate
    };
  }
}
