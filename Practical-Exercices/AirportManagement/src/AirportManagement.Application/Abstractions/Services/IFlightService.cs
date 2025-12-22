using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Flight;

namespace AirportManagement.Application.Abstractions.Services;

public interface IFlightService
{
    Task<FlightResponseDto> GetByIdAsync(int id, CancellationToken ct);

    Task<FlightResponseDto?> GetByIdWithRelatedDataAsync(int id, CancellationToken ct);

    Task<PagedResponse<FlightListItemResponse>> SearchAsync(FlightSearchQuery query, CancellationToken ct);

    Task<Result<FlightResponseDto>> CreateAsync(CreateFlightRequest request, CancellationToken ct);
}
