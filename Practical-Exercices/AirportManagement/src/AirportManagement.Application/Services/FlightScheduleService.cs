using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Mappings;
using AirportManagement.Domain;
using AirportManagement.Domain.Models;
using System.Text.Json;

namespace AirportManagement.Application.Services;

public class FlightScheduleService : IFlightScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlightScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ScheduleResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var schedule = await _unitOfWork.FlightSchedules.GetByIdAsync(id, ct);

        if (schedule is null)
        {
            return null;
        }

        return schedule.MapToScheduleResponse();
    }

    public async Task<Result<ImportSchedulesResponseDto>> ImportAsync(Stream jsonStream, CancellationToken ct)
    {
        List<ImportScheduleRowDto>? rows;

        try
        {
            rows = await JsonSerializer.DeserializeAsync<List<ImportScheduleRowDto>>(
                jsonStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                ct);
        }
        catch (JsonException)
        {
            return Result<ImportSchedulesResponseDto>.Fail(ErrorType.Validation, "Invalid JSON file.");
        }

        if (rows is null || rows.Count == 0)
            return Result<ImportSchedulesResponseDto>.Fail(ErrorType.Validation, "JSON file contains no rows.");

        var response = new ImportSchedulesResponseDto { Total = rows.Count };

        var flightCache = new Dictionary<int, Flight>();
        var aircraftCache = new Dictionary<string, Aircraft>(StringComparer.OrdinalIgnoreCase);
        var gateCache = new Dictionary<(int airportId, string code), Gate>();

        for (var i = 0; i < rows.Count; i++)
        {
            var rowNumber = i + 1;
            var row = rows[i];

            if (row.FlightId <= 0)
            {
                response.Errors.Add(new ImportRowErrorDto { Row = rowNumber, Message = "flightId is required." });
                continue;
            }

            if (row.ScheduledDepartureUtc >= row.ScheduledArrivalUtc)
            {
                response.Errors.Add(new ImportRowErrorDto { Row = rowNumber, Message = "Departure must be earlier than arrival." });
                continue;
            }

            if (row.Status > 4)
            {
                response.Errors.Add(new ImportRowErrorDto { Row = rowNumber, Message = $"Invalid status value: {row.Status}." });
                continue;
            }

            if (!flightCache.TryGetValue(row.FlightId, out var flight))
            {
                var found = await _unitOfWork.Flights.GetByIdAsync(row.FlightId, ct);
                if (found is null)
                {
                    response.Errors.Add(new ImportRowErrorDto { Row = rowNumber, Message = $"Flight with id {row.FlightId} not found." });
                    continue;
                }

                flight = found;
                flightCache[row.FlightId] = flight;
            }

            int? gateId = null;
            if (!string.IsNullOrWhiteSpace(row.GateCode))
            {
                var gateCode = row.GateCode.Trim();
                var key = (airportId: flight.OriginAirportId, code: gateCode);

                if (!gateCache.TryGetValue(key, out var gate))
                {
                    var foundGate = await _unitOfWork.Gates.GetByAirportAndCodeAsync(key.airportId, key.code, ct);
                    if (foundGate is null)
                    {
                        response.Errors.Add(new ImportRowErrorDto
                        {
                            Row = rowNumber,
                            Message = $"Gate not found: AirportId={key.airportId}, Code={key.code}."
                        });
                        continue;
                    }

                    gate = foundGate;
                    gateCache[key] = gate;
                }

                gateId = gate.Id;
            }

            int? aircraftId = null;
            if (!string.IsNullOrWhiteSpace(row.AssignedAircraftTail))
            {
                var tail = row.AssignedAircraftTail.Trim();

                if (!aircraftCache.TryGetValue(tail, out var aircraft))
                {
                    var foundAircraft = await _unitOfWork.Aircrafts.GetByTailNumberAsync(tail, ct);
                    if (foundAircraft is null)
                    {
                        response.Errors.Add(new ImportRowErrorDto { Row = rowNumber, Message = $"Aircraft not found: Tail={tail}." });
                        continue;
                    }

                    aircraft = foundAircraft;
                    aircraftCache[tail] = aircraft;
                }

                aircraftId = aircraft.Id;
            }

            var existing = await _unitOfWork.FlightSchedules
                .GetByFlightIdAndDepartureAsync(row.FlightId, row.ScheduledDepartureUtc, ct);

            if (gateId is not null)
            {
                var overlap = await _unitOfWork.FlightSchedules.ExistsGateOverlapAsync(
                    gateId.Value,
                    row.ScheduledDepartureUtc,
                    row.ScheduledArrivalUtc,
                    excludeScheduleId: existing?.Id,
                    ct);

                if (overlap)
                {
                    response.Errors.Add(new ImportRowErrorDto
                    {
                        Row = rowNumber,
                        Message = $"Gate overlap at AirportId={flight.OriginAirportId}:{row.GateCode} {row.ScheduledDepartureUtc:O}–{row.ScheduledArrivalUtc:O}"
                    });
                    continue;
                }
            }

            var statusEnum = (FlightScheduleStatus)row.Status;

            if (existing is null)
            {
                var schedule = new FlightSchedule
                {
                    FlightId = row.FlightId,
                    ScheduledDepartureUtc = row.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = row.ScheduledArrivalUtc,
                    GateId = gateId,
                    AssignedAircraftId = aircraftId,
                    Status = statusEnum
                };

                await _unitOfWork.FlightSchedules.InsertAsync(schedule, ct);
                response.Created++;
            }
            else
            {
                existing.ScheduledArrivalUtc = row.ScheduledArrivalUtc;
                existing.GateId = gateId;
                existing.AssignedAircraftId = aircraftId;
                existing.Status = statusEnum;

                await _unitOfWork.FlightSchedules.UpdateAsync(existing, ct);
                response.Updated++;
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Result<ImportSchedulesResponseDto>.Ok(response);
    }

    public async Task<PagedResponse<ScheduleListItemResponse>> SearchAsync(ScheduleSearchQuery query, CancellationToken ct)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _unitOfWork.FlightSchedules.SearchAsync(
            query.Origin,
            query.Destination,
            query.Date,
            page,
            pageSize,
            ct);

        return new PagedResponse<ScheduleListItemResponse> 
        {
            Items = items, 
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct)
    {
        var schedule = await _unitOfWork.FlightSchedules.GetByIdAsync(id, ct);
        if (schedule is null)
            return Result.Fail(ErrorType.NotFound, $"Schedule {id} not found.");

        var hasTickets = await _unitOfWork.FlightSchedules.HasTicketsAsync(id, ct);
        if (hasTickets)
            return Result.Fail(ErrorType.Conflict, "Cannot delete schedule because tickets exist.");

        await _unitOfWork.FlightSchedules.DeleteAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
