using API.DTOs;
using AutoMapper;
using Core.Entities;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, AppUser>();

        CreateMap<CreateStudentDto, AppUser>();

        CreateMap<StudentUser, StudentDto>()
            .ForMember(x => x.Email, opt => opt.MapFrom(src => src.AppUser.Email))
            .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.AppUser.UserName))
            .ForMember(x => x.Photos, opt => opt.MapFrom(src => src.AppUser.Photos));

        CreateMap<UpdateStudentDto, AppUser>();


        CreateMap<CreateSubjectDto, Subject>();

        CreateMap<Subject, SubjectDto>();

        CreateMap<Photo, PhotoDto>();

        CreateMap<Message, MessageDto>();
    }
}
