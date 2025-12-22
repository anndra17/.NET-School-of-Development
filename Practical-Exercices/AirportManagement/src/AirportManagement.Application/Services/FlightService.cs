using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Mappings;

namespace AirportManagement.Application.Services;

public class FlightService : IFlightService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlightService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _unitOfWork.Flights.GetByIdAsync(id, ct);
        
        if (entity is null)
        {
            return null;
        }

        return entity.MapToFlightResponse();
    }
}
