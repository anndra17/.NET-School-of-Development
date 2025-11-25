using AutoMapper;
using Cms.Repository.DTOs;
using Cms.Repository.Models;
using Cms.Repository.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICmsRepository _repository;
    private readonly IMapper _mapper;

    public CoursesController(ICmsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    #region Return type approaches
    // Return type - Approach 1 - primitive or complex type
    //Approach 1: Using LINQ Select to map Course to CourseDto
    //[HttpGet]
    //public IEnumerable<Course> GetCoureses()
    //{
    //    return _repository.GetAllCourses();
    //}


    // Return type - Approach 1 - primitive or complex type
    //[HttpGet]
    //public IEnumerable<CourseDto> GetCoureses()
    //{
    //    try
    //    {
    //        IEnumerable<Course> courses = _repository.GetAllCourses();

    //        var result = MapCourseToCourseDto(courses);

    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    // Return type - Aproach 2 - use IActionResult
    //[HttpGet]
    //public IActionResult GetCoureses()
    //{
    //    try
    //    {
    //        IEnumerable<Course> courses = _repository.GetAllCourses();

    //        var result = MapCourseToCourseDto(courses);

    //        return Ok(result);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    //    }
    //}

    // Return type - Approach 4 - Async Task<T>
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<CourseDto>>> GetCouresesAsync()
    //{
    //    try
    //    {
    //        IEnumerable<Course> courses = await _repository.GetAllCoursesAsync();

    //        var result = MapCourseToCourseDto(courses);

    //        return result.ToList(); // Convert to support IActionResult<T>
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    //    }
    //}
    #endregion

    // Return type - Approach 3 - ActionResult<T>
    [HttpGet]
    public ActionResult<List<CourseDto>> GetCoureses()
    {
        try
        {
            IEnumerable<Course> courses = _repository.GetAllCourses();

            var result = _mapper.Map<CourseDto[]>(courses);

            return result.ToList(); 
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    #region Custom mapper functions
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
    #endregion
}
