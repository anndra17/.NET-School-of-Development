using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Common.Validators;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Enums;
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
        var entity = await _unitOfWork.FlightsRepository.GetByIdAsync(id, ct);
        
        if (entity is null)
        {
            return null;
        }

        return entity.MapToFlightResponse();
    }

    public async Task<FlightResponseDto?> GetByIdWithRelatedDataAsync(int id, CancellationToken ct)
    {
        return await _unitOfWork.FlightsRepository.GetByIdWithRelatedDataAsync(id, ct);
    }

    public async Task<Result<FlightResponseDto>> CreateAsync(CreateFlightRequest request, CancellationToken ct)
    {
        if (request.OriginAirportId == request.DestinationAirportId)
            return Result<FlightResponseDto>.Fail(
                ErrorType.Validation, 
                "Origin and Destination cannot be the same airport."
            );

        var airlineExists = await _unitOfWork.AirlinesRepository.ExistsAsync(request.AirlineId, ct);
        if (!airlineExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Airline {request.AirlineId} not found."
            );

        var originExists = await _unitOfWork.AirportsRepository.ExistsAsync(request.OriginAirportId, ct);
        if (!originExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Origin airport {request.OriginAirportId} not found."
            );

        var destExists = await _unitOfWork.AirportsRepository.ExistsAsync(request.DestinationAirportId, ct);
        if (!destExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Destination airport {request.DestinationAirportId} not found."
            );

        if (request.DefaultAircraftId is not null)
        {
            var aircraftExists = await _unitOfWork.AircraftsRepository.ExistsAsync(request.DefaultAircraftId.Value, ct);
            if (!aircraftExists)
                return Result<FlightResponseDto>.Fail(
                    ErrorType.NotFound, 
                    $"Aircraft {request.DefaultAircraftId.Value} not found."
                );
        }

        var conflict = await _unitOfWork.FlightsRepository.ExistsByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        if (conflict)
            return Result<FlightResponseDto>.Fail(
                ErrorType.Conflict, 
                "A flight with the same AirlineId and FlightNumber already exists."
            );

        var entity = request.MapToDomain();

        await _unitOfWork.FlightsRepository.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);
        

        var created = await _unitOfWork.FlightsRepository.GetByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        return Result<FlightResponseDto>.Ok(created?.MapToFlightResponse() ?? entity.MapToFlightResponse());
    }

    public async Task<Result<FlightResponseDto>> UpdateAsync(int id, UpdateFlightRequest request, CancellationToken ct)
    {
        if (request.OriginAirportId == request.DestinationAirportId)
            return Result<FlightResponseDto>.Fail(ErrorType.Validation, "Origin and Destination cannot be the same airport.");

        var entity = await _unitOfWork.FlightsRepository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Flight {id} not found.");

        if (!await _unitOfWork.AirlinesRepository.ExistsAsync(request.AirlineId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Airline {request.AirlineId} not found.");

        if (!await _unitOfWork.AirportsRepository.ExistsAsync(request.OriginAirportId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Origin airport {request.OriginAirportId} not found.");

        if (!await _unitOfWork.AirportsRepository.ExistsAsync(request.DestinationAirportId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Destination airport {request.DestinationAirportId} not found.");

        if (request.DefaultAircraftId is not null &&
            !await _unitOfWork.AircraftsRepository.ExistsAsync(request.DefaultAircraftId.Value, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Aircraft {request.DefaultAircraftId.Value} not found.");

        var conflict = await _unitOfWork.FlightsRepository.ExistsByAirlineAndNumberExceptAsync(request.AirlineId, request.FlightNumber, id, ct);
        if (conflict)
            return Result<FlightResponseDto>.Fail(ErrorType.Conflict, "Another flight with the same AirlineId and FlightNumber already exists.");

        request.ApplyToDomain(entity);

        await _unitOfWork.FlightsRepository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.FlightsRepository.GetByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        return Result<FlightResponseDto>.Ok(updated?.MapToFlightResponse()?? entity.MapToFlightResponse());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _unitOfWork.FlightsRepository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Fail(ErrorType.NotFound, $"Flight {id} not found.");

        var hasDependencies = await _unitOfWork.FlightsRepository.HasSchedulesAsync(id, ct); 
        if (hasDependencies)
            return Result.Fail(ErrorType.Conflict, "Cannot delete flight because schedules exists. Deactivate it instead.");

        await _unitOfWork.FlightsRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
