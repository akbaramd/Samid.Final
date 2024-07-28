using AutoMapper;
using Samid.Application.DTOs;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<User, UserDto>()
      .ForMember(x => x.UserAcademicYears, v => v.MapFrom(b => b.UserAcademicYears));
    CreateMap<UserAcademicYear, UserAcademicYearDto>();
    CreateMap<AcademicYear, AcademicYearDto>();
    CreateMap<GradeFieldOfStudy, GradeFieldOfStudyDto>();
    CreateMap<GradeOfStudy, GradeOfStudyDto>();
    CreateMap<FieldOfStudy, FieldOfStudyDto>();
  }
}
