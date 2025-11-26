using Cms.Repository.Models;

namespace Cms.Repository.Repositories;

public interface ICmsRepository
{
    // Collection
    IEnumerable<Course> GetAllCourses();
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Course AddCourse(Course newCourse);
    bool IfCourseExists(int courseId);

    // Individual item
    Course GetCourseById(int courseId);
    Course UpdateCourse(int courseId, Course course);
    Course RemoveCourse(int courseId);

    // Association
    IEnumerable<Student> GetStudents(int courseId);
    //Student AddStudent(int courseId, Student student);
}
