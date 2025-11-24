using Cms.Repository.DTOs;
using Cms.Repository.Models;
using Cms.Repository.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Cms.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICmsRepository _repository;

    public CoursesController(ICmsRepository repository)
    {
        _repository = repository;
    }

    //Approach 1: Using LINQ Select to map Course to CourseDto
    //[HttpGet]
    //public IEnumerable<Course> GetCoureses()
    //{
    //    return _repository.GetAllCourses();
    //}

    [HttpGet]
    public IEnumerable<CourseDto> GetCoureses()
    {
        try
        {
            IEnumerable<Course> courses = _repository.GetAllCourses();

            var result = MapCourseToCourseDto(courses);

            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    //Custom mapper functions
    private CourseDto MapCourseToCourseDto(Course course)
    {
        return new CourseDto()
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            CourseDuration = course.CourseDuration,
            CourseType = course.CourseType
        };
    }

    private IEnumerable<CourseDto> MapCourseToCourseDto(IEnumerable<Course> course)
    {
        IEnumerable<CourseDto> result;

        result = course.Select(c => new CourseDto() 
        { 
            CourseId = c.CourseId,
            CourseName = c.CourseName,
            CourseDuration = c.CourseDuration,
            CourseType = c.CourseType
        });

        return result;
    }
}
