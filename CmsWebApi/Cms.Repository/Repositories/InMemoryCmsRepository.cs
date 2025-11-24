using Cms.Repository.Models;

namespace Cms.Repository.Repositories; 

public class InMemoryCmsRepository : ICmsRepository
{
    public InMemoryCmsRepository()
    {

    }

    public IEnumerable<Course>  GetAllCourses()
    {
        return null;
    }
}
