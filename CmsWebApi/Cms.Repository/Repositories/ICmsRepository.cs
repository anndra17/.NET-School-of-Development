using Cms.Repository.Models;

namespace Cms.Repository.Repositories;

public interface ICmsRepository
{
    IEnumerable<Course> GetAllCourses();
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Course AddCourse(Course newCourse);
    bool IfCourseExists(int courseId);
    Course GetCourseById(int courseId);
    Course UpdateCourse(int courseId, Course course);
}
