using Cms.Repository.Models;

namespace Cms.Repository.Repositories;

public class SqlCmsRepository : ICmsRepository
{
    public SqlCmsRepository()
    {
    }

    public IEnumerable<Course> GetAllCourses()
    {
        return null;
    }
}
