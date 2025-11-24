using Cms.Repository.Models;

namespace Cms.Repository.Repositories; 

public class InMemoryCmsRepository : ICmsRepository
{
    List<Course> _courses = null;
    public InMemoryCmsRepository()
    {
        _courses = new List<Course>();
        _courses.Add(
            new Course() 
            { 
                CourseId = 1, 
                CourseName = "Computer Science", 
                CourseDuration = 4, 
                CourseType = COURSE_TYPE.ENIGINEERING 
            }
        );
        _courses.Add(
            new Course() { 
                CourseId = 2, 
                CourseName = "Mechanical Engineering", 
                CourseDuration = 4, 
                CourseType = COURSE_TYPE.ENIGINEERING 
            }
        );
        _courses.Add(
            new Course() 
            {   
                CourseId = 3, 
                CourseName = "Civil Engineering", 
                CourseDuration = 4, 
                CourseType = COURSE_TYPE.ENIGINEERING 
            }
        );
        _courses.Add(
            new Course() 
            { 
                CourseId = 4, 
                CourseName = "MBBS", 
                CourseDuration = 5, 
                CourseType = COURSE_TYPE.MEDICAL 
            }
        );
        _courses.Add(
            new Course() 
            { 
                CourseId = 5, 
                CourseName = "BDS", 
                CourseDuration = 5, 
                CourseType = COURSE_TYPE.MEDICAL 
            }
        );
    }

    public IEnumerable<Course>  GetAllCourses()
    {
        return _courses;
    }
}
