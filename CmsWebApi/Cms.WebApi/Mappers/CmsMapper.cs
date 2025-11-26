using AutoMapper;
using Cms.Repository.DTOs;
using Cms.Repository.Models;

namespace Cms.WebApi.Mappers;

public class CmsMapper : Profile
{
    public CmsMapper()
    {
        CreateMap<CourseDto, Course>()
            .ReverseMap();
        CreateMap<StudentDto, Student>()
            .ReverseMap();
    }
}
