using Cms.Repository.Models;

namespace Cms.Repository.Repositories; 

public class InMemoryCmsRepository : ICmsRepository
{
    List<Course> _courses = null;
    List<Student> _students = null;
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

        _students = new List<Student>();
        _students.Add(
            new Student()
            {
                StudentId = 101,
                FirstName = "James",
                LastName = "Smith",
                PhoneNumber = "555-555-1234",
                Address = "US",
                Course = _courses.Where(c => c.CourseId == 1).SingleOrDefault()
            }
        );
        _students.Add(
            new Student()
            {
                StudentId = 102,
                FirstName = "Ana",
                LastName = "Moore",
                PhoneNumber = "333-333-1234",
                Address = "RO",
                Course = _courses.Where(c => c.CourseId == 1).SingleOrDefault()
            }
        );
    }

    public IEnumerable<Course>  GetAllCourses()
    {
        return _courses;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await Task.Run(() => _courses.ToList());
    }

    public bool IfCourseExists(int courseId)
    {
        return _courses.Any(c => c.CourseId == courseId);
    }

    public Course GetCourseById(int courseId)
    {
        var result = _courses.Where(c => c.CourseId == courseId)
                             .SingleOrDefault();

        return result;
    }

    public Course AddCourse(Course newCourse)
    {
        var maxCourseId = _courses.Max(c => c.CourseId);
        newCourse.CourseId = ++maxCourseId;
        _courses.Add(newCourse);

        return newCourse;
    }

    public Course UpdateCourse(int courseId, Course course)
    {
        var result = _courses.Where(c => c.CourseId == courseId)
                             .SingleOrDefault();

        if (result != null)
        {
            result.CourseName = course.CourseName;
            result.CourseDuration = course.CourseDuration;
            result.CourseType = course.CourseType;
        }

        return result;
    }

    public Course RemoveCourse(int courseId)
    {
        var result = _courses.Where(c => c.CourseId == courseId)
                             .SingleOrDefault();
        if (result != null)
        {
            _courses.Remove(result);
        }

        return result;
    }

    public IEnumerable<Student> GetStudents(int courseId)
    {
        return _students.Where(s => s.Course.CourseId == courseId);
    }

    public Student AddStudent(Student newStudent)
    {
        var maxStudentId = _students.Max(s => s.StudentId);
        newStudent.StudentId = ++maxStudentId;
        _students.Add(newStudent);

        return newStudent;
    }
}
