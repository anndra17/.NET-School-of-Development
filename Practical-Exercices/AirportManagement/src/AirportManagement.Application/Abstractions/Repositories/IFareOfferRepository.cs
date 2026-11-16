using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;

namespace AirportManagement.Application.Abstractions.Repositories;

public interface IFareOfferRepository
{
    Task<FareOffer?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<FareOffer?> GetByScheduleAndClassAsync(int flightScheduleId, FareClass fareClass, CancellationToken ct = default);

    Task<IReadOnlyList<FareOffer>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct = default);

    Task<bool> TryReserveSeatsAsync(int fareOfferId, int quantity, CancellationToken ct = default);

    Task RestoreSeatsAsync(int fareOfferId, int quantity, CancellationToken ct = default);
}
