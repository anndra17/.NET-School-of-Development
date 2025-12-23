using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Booking;

namespace AirportManagement.Application.Abstractions.Services;

public interface IBookingService
{
    Task<Result<CreateBookingResponseDto>> CreateAsync(CreateBookingRequest request, CancellationToken ct);
    Task<BookingResponseDto?> GetByCodeAsync(string code, CancellationToken ct);
    Task<Result> CancelAsync(string code, CancellationToken ct);
}