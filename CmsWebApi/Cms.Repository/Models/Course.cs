namespace Cms.Repository.Models;

public class Course
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int CourseDuration { get; set; }
    public COURSE_TYPE CourseType { get; set; }
}

public enum COURSE_TYPE
{
    ENIGINEERING,
    MEDICAL, 
    MANAGEMENT,
}