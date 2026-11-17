using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Ticket;

namespace AirportManagement.Application.Abstractions.Services;

public interface ITicketService
{
    Task<TicketResponseDto?> GetByIdAsync(long id, CancellationToken ct);

    Task<Result<TicketResponseDto>> CreateAsync(CreateTicketRequest request, CancellationToken ct);

    Task<Result> DeleteAsync(long id, CancellationToken ct);

    Task<Result<TicketAvailabilityResponseDto>> GetAvailabilityAsync(int flightScheduleId, CancellationToken ct);

    Task<IReadOnlyList<TicketResponseDto>> GetByScheduleAsync(int flightScheduleId, CancellationToken ct);

    Task<Result<TicketResponseDto>> UpdateInventoryAsync(long id, UpdateTicketInventoryRequest request, CancellationToken ct);
}
