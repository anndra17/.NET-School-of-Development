namespace AirportManagement.Application.Dtos.Ticket;

public sealed class FareClassPriceDto
{
    public string FareClass { get; init; } = null!;
    public decimal? MinTotalPrice { get; init; }
    public string? Currency { get; init; }
}