using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Mappings;
using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class FareOfferRepository : IFareOfferRepository
{
    private readonly AirportManagementDbContext _context;

    public FareOfferRepository(AirportManagementDbContext context)
    {
        _context = context;
    }

    public async Task<FareOffer?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Set<FareOfferEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<FareOffer>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct = default)
    {
        var entities = await _context.Set<FareOfferEntity>()
            .AsNoTracking()
            .Where(x => x.FlightScheduleId == flightScheduleId)
            .OrderBy(x => x.FareClass)
            .ToListAsync(ct);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<FareOffer?> GetByScheduleAndClassAsync(int flightScheduleId, FareClass fareClass, CancellationToken ct = default)
    {
        var code = FareClassMappings.ToEntity(fareClass); // "Y/M/J/F"

        var entity = await _context.Set<FareOfferEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FlightScheduleId == flightScheduleId && x.FareClass == code, ct);

        return entity?.ToDomain();
    }

    public async Task<bool> TryReserveSeatsAsync(int fareOfferId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be > 0.");

        var rowsAffected = await _context.Set<FareOfferEntity>()
            .Where(x => x.Id == fareOfferId && x.SeatsAvailable >= quantity)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SeatsAvailable, x => x.SeatsAvailable - quantity),
                ct);

        return rowsAffected == 1;
    }

    public async Task RestoreSeatsAsync(int fareOfferId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be > 0.");

        await _context.Set<FareOfferEntity>()
            .Where(x => x.Id == fareOfferId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SeatsAvailable, x => x.SeatsAvailable + quantity),
                ct);
    }
}