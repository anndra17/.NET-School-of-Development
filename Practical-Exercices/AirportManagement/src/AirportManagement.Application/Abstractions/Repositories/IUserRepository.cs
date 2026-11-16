using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, int>
{
}
