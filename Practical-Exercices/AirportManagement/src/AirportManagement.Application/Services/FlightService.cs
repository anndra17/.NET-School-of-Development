using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Mappings;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Common.Validators;
using AirportManagement.Domain.Models;
using System.Data.Common;


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

    public async Task<FlightResponseDto?> GetByIdWithRelatedDataAsync(int id, CancellationToken ct)
    {
        return await _unitOfWork.Flights.GetByIdWithRelatedDataAsync(id, ct);
    }

    public async Task<PagedResponse<FlightListItemResponse>> SearchAsync(FlightSearchQuery query, CancellationToken ct)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 ? 20 : query.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _unitOfWork.Flights.SearchAsync(
            query.AirlineId, query.OriginAirportId, query.DestinationAirportId,
            query.FlightNumber, query.IsActive, page, pageSize, ct);

        return new PagedResponse<FlightListItemResponse>
        {
            Items = items, // deja DTO
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<Result<FlightResponseDto>> CreateAsync(CreateFlightRequest request, CancellationToken ct)
    {
        if (request.OriginAirportId == request.DestinationAirportId)
            return Result<FlightResponseDto>.Fail(
                ErrorType.Validation, 
                "Origin and Destination cannot be the same airport."
            );

        if (!request.FlightNumber.IsValidFlightNumber())
            return Result<FlightResponseDto>.Fail(
                ErrorType.Validation, 
                "FlightNumber must be 2 uppercase letters followed by 4 digits (e.g. RO1234)."
            );

        var airlineExists = await _unitOfWork.Airlines.ExistsAsync(request.AirlineId, ct);
        if (!airlineExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Airline {request.AirlineId} not found."
            );

        var originExists = await _unitOfWork.Airports.ExistsAsync(request.OriginAirportId, ct);
        if (!originExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Origin airport {request.OriginAirportId} not found."
            );

        var destExists = await _unitOfWork.Airports.ExistsAsync(request.DestinationAirportId, ct);
        if (!destExists)
            return Result<FlightResponseDto>.Fail(
                ErrorType.NotFound, 
                $"Destination airport {request.DestinationAirportId} not found."
            );

        if (request.DefaultAircraftId is not null)
        {
            var aircraftExists = await _unitOfWork.Aircrafts.ExistsAsync(request.DefaultAircraftId.Value, ct);
            if (!aircraftExists)
                return Result<FlightResponseDto>.Fail(
                    ErrorType.NotFound, 
                    $"Aircraft {request.DefaultAircraftId.Value} not found."
                );
        }

        var conflict = await _unitOfWork.Flights.ExistsByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        if (conflict)
            return Result<FlightResponseDto>.Fail(
                ErrorType.Conflict, 
                "A flight with the same AirlineId and FlightNumber already exists."
            );

        var entity = new Flight
        {
            AirlineId = request.AirlineId,
            FlightNumber = request.FlightNumber,
            OriginAirportId = request.OriginAirportId,
            DestinationAirportId = request.DestinationAirportId,
            DefaultAircraftId = request.DefaultAircraftId,
            IsActive = request.IsActive ?? true
        };

        await _unitOfWork.Flights.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);
        

        var created = await _unitOfWork.Flights.GetByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        return Result<FlightResponseDto>.Ok(created?.MapToFlightResponse() ?? entity.MapToFlightResponse());
    }

    public async Task<Result<FlightResponseDto>> UpdateAsync(int id, UpdateFlightRequest request, CancellationToken ct)
    {
        if (request.OriginAirportId == request.DestinationAirportId)
            return Result<FlightResponseDto>.Fail(ErrorType.Validation, "Origin and Destination cannot be the same airport.");

        if (!request.FlightNumber.IsValidFlightNumber())
            return Result<FlightResponseDto>.Fail(ErrorType.Validation, "FlightNumber must be 2 uppercase letters followed by 4 digits (e.g. RO1234).");

        var entity = await _unitOfWork.Flights.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Flight {id} not found.");

        if (!await _unitOfWork.Airlines.ExistsAsync(request.AirlineId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Airline {request.AirlineId} not found.");

        if (!await _unitOfWork.Airports.ExistsAsync(request.OriginAirportId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Origin airport {request.OriginAirportId} not found.");

        if (!await _unitOfWork.Airports.ExistsAsync(request.DestinationAirportId, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Destination airport {request.DestinationAirportId} not found.");

        if (request.DefaultAircraftId is not null &&
            !await _unitOfWork.Aircrafts.ExistsAsync(request.DefaultAircraftId.Value, ct))
            return Result<FlightResponseDto>.Fail(ErrorType.NotFound, $"Aircraft {request.DefaultAircraftId.Value} not found.");

        var conflict = await _unitOfWork.Flights.ExistsByAirlineAndNumberExceptAsync(request.AirlineId, request.FlightNumber, id, ct);
        if (conflict)
            return Result<FlightResponseDto>.Fail(ErrorType.Conflict, "Another flight with the same AirlineId and FlightNumber already exists.");

        await _unitOfWork.Flights.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.Flights.GetByAirlineAndNumberAsync(request.AirlineId, request.FlightNumber, ct);
        return Result<FlightResponseDto>.Ok(updated?.MapToFlightResponse()?? entity.MapToFlightResponse());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _unitOfWork.Flights.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Fail(ErrorType.NotFound, $"Flight {id} not found.");

        var hasDependencies = await _unitOfWork.Flights.HasSchedulesAsync(id, ct); 
        if (hasDependencies)
            return Result.Fail(ErrorType.Conflict, "Cannot delete flight because schedules exists. Deactivate it instead.");

        await _unitOfWork.Flights.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
