using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IAirlineRepository : IRepository<Airline, int>
{
}
