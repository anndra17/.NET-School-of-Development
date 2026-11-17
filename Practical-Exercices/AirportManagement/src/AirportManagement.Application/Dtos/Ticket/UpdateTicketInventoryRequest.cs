using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Ticket;

public sealed class UpdateTicketInventoryRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "SeatInventory must be >= 0.")]
    public int SeatInventory { get; init; }
}