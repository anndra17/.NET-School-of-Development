using AutoMapper;
using Cms.Repository.DTOs;
using Cms.Repository.Models;
using Cms.Repository.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace Cms.WebApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("[controller]")]
public class Courses2Controller : ControllerBase
{
    private readonly ICmsRepository _repository;
    private readonly IMapper _mapper;

    public Courses2Controller(ICmsRepository repository, IMapper mapper)
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
    #endregion

    // Return type - Approach 3 - ActionResult<T>
    [HttpGet]
    public ActionResult<List<CourseDto>> GetCoureses()
    {
        try
        {
            IEnumerable<Course> courses = _repository.GetAllCourses();

            var result = _mapper.Map<CourseDto[]>(courses);

            // version 2 changes
            foreach (var item in result)
            {
                item.CourseName += " (v2.0)";
            }

            return result.ToList(); 
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public ActionResult<CourseDto> AddCourse([FromBody] CourseDto course)
    {
        try
        {
            var newCourse = _mapper.Map<Course>(course);

            newCourse = _repository.AddCourse(newCourse);

            return _mapper.Map<CourseDto>(newCourse);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("courseId")]
    public ActionResult<CourseDto> GetCourse(int courseId)
    {
        try
        {
            if (!_repository.IfCourseExists(courseId))
                return NotFound();

            var searchedCourse = _repository.GetCourseById(courseId);

            return _mapper.Map<CourseDto>(searchedCourse);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("courseId")]
    public ActionResult<CourseDto> UpdateCourse(int courseId, CourseDto course)
    {
        try
        {
            if (!_repository.IfCourseExists(courseId))
                return NotFound();

            var updatedCourse = _mapper.Map<Course>(course);
            updatedCourse = _repository.UpdateCourse(courseId, updatedCourse);

            return _mapper.Map<CourseDto>(updatedCourse);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("courseId")]
    public ActionResult<CourseDto> RemoveCourse(int courseId)
    {
        try
        {
            if (!_repository.IfCourseExists(courseId))
                return NotFound();

            var course = _repository.RemoveCourse(courseId);

            if (course == null)
                return BadRequest();

            return _mapper.Map<CourseDto>(course);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // Association
    [HttpGet("courseId/students")]
    public ActionResult<IEnumerable<StudentDto>> GetStudents(int courseId)
    {
        try
        {
            if (!_repository.IfCourseExists(courseId))
                return NotFound();

            IEnumerable<Student> students = _repository.GetStudents(courseId);
            var result = _mapper.Map<StudentDto[]>(students);

            return result;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("{courseId}/students")]
    public ActionResult<StudentDto> AddStudent(int courseId, [FromBody] StudentDto student)
    {
        try
        {
            if (!_repository.IfCourseExists(courseId))
                return NotFound();

            var newStudent = _mapper.Map<Student>(student);

            var course = _repository.GetCourseById(courseId);
            newStudent.Course = course;

            newStudent = _repository.AddStudent(newStudent);
            var result = _mapper.Map<StudentDto>(newStudent);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    #region Async methods
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
