using AutoMapper;
using Samid.Application.DTOs;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<User, UserDto>()
      .ForMember(x => x.UserEducationMajors, v => v.MapFrom(b => b.UserEducationMajors));
    CreateMap<UserEducationMajors, UserEducationMajorsDto>();
    CreateMap<AcademicYear, AcademicYearDto>();
    CreateMap<EducationMajors, EducationMajorsDto>();
    CreateMap<EducationGrade, EducationGradeDto>();
    CreateMap<EducationField, EducationFieldDto>();
    CreateMap<EducationBook, EducationBookDto>();
    CreateMap<EducationStage, EducationStageDto>();
  }
}
