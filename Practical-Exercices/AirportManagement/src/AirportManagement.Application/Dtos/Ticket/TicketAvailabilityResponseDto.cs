namespace AirportManagement.Application.Dtos.Ticket;

public sealed class TicketAvailabilityResponseDto
{
    public int FlightScheduleId { get; init; }

    public int Capacity { get; init; }
    public int TicketsCount { get; init; }
    public int SeatsRemaining { get; init; }

    public IReadOnlyList<FareClassPriceDto> Prices { get; init; } = Array.Empty<FareClassPriceDto>();
}
