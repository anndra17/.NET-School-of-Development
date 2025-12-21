using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        public BookingRepository(AirportManagementDbContext context) 
        {
        }
    }
}
