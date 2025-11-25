using Cms.Repository.Models;

namespace Cms.Repository.Repositories;

public interface ICmsRepository
{
    IEnumerable<Course> GetAllCourses();
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Course AddCourse(Course newCourse);

}
