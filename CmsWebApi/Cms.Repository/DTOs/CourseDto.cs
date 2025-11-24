using Cms.Repository.Models;

namespace Cms.Repository.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int CourseDuration { get; set; }
    public COURSE_TYPE CourseType { get; set; }
}
