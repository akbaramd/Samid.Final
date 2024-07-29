using AutoMapper;
using Samid.Application.DTOs;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<User, UserDto>()
      .ForMember(x => x.UserStudyMajors, v => v.MapFrom(b => b.UserStudyMajors));
    CreateMap<UserStudyMajors, UserStudyMajorsDto>();
    CreateMap<AcademicYear, AcademicYearDto>();
    CreateMap<StudyMajors, StudyMajorsDto>();
    CreateMap<StudyGrade, StudyGradeDto>();
    CreateMap<StudyField, StudyFieldDto>();
    CreateMap<StudyBook, StudyBookDto>();
  }
}
