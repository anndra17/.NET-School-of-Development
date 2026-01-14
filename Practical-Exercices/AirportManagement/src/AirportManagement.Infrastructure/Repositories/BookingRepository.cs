using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
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

        public async Task InsertAsync(Booking booking, CancellationToken ct = default)
        {
            var entity = new BookingEntity
            {
                UserId = booking.UserId,
                ConfirmationCode = booking.ConfirmationCode,
                Quantity = booking.Quantity,
                Status = (byte)booking.Status,
                CreatedUtc = booking.CreatedUtc
            };

            await _context.Set<BookingEntity>().AddAsync(entity, ct);
        }

        public Task UpdateAsync(Booking entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Bookings.AnyAsync(f => f.Id == id, ct);
        }

        public async Task<Booking?> GetByCodeAsync(string confirmationCode, CancellationToken ct = default)
        {
            var code = confirmationCode.Trim().ToUpperInvariant();

            var entity = await _context.Set<BookingEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ConfirmationCode == code, ct);

            return entity?.ToDomain();
        }

        public Task<bool> ExistsByCodeAsync(string confirmationCode, CancellationToken ct = default)
        {
            var code = confirmationCode.Trim().ToUpperInvariant();

            return _context.Set<BookingEntity>()
                .AsNoTracking()
                .AnyAsync(b => b.ConfirmationCode == code, ct);
        }

        public async Task CancelByCodeAsync(string confirmationCode, CancellationToken ct = default)
        {
            var code = confirmationCode.Trim().ToUpperInvariant();

            var tracked = await _context.Set<BookingEntity>()
                .FirstOrDefaultAsync(b => b.ConfirmationCode == code, ct);

            if (tracked is null)
                return;

            tracked.Status = (byte)BookingStatus.Cancelled;
        }
    }
}
