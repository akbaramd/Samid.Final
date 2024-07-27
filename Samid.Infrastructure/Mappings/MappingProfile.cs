using AutoMapper;
using Samid.Application.DTOs.Authentication;

namespace Samid.Inrastructure.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<User, AuthUserProfileResponse>()
      .ForMember(x => x.UserId, b => b.MapFrom(x => x.Id));
  }
}
