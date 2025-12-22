using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AirportManagementDbContext _context;

        public BookingRepository(AirportManagementDbContext context)
        {
            _context = context;
        }

        public Task DeleteAsync(int Id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Booking?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Booking entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Booking entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Bookings.AnyAsync(f => f.Id == id, ct);
        }
    }
}
