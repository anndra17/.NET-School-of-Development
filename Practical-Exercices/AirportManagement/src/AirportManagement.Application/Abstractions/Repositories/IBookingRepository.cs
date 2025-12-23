using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IBookingRepository : IRepository<Booking, int>
{
    Task<Booking?> GetByCodeAsync(string confirmationCode, CancellationToken ct = default);

    Task<bool> ExistsByCodeAsync(string confirmationCode, CancellationToken ct = default);

    Task InsertAsync(Booking booking, CancellationToken ct = default);

    Task CancelByCodeAsync(string confirmationCode, CancellationToken ct = default);
}
