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

        public Task DeleteAsync(object Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Booking>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Booking?> GetByIdAsync(object Id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Booking entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Booking entity)
        {
            throw new NotImplementedException();
        }
    }
}
