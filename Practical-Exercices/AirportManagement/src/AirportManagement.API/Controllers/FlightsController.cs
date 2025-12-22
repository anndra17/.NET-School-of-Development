using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    // GET /api/flights/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FlightResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlightResponseDto>> GetById(int id, CancellationToken ct)
    {
        var flight = await _flightService.GetByIdAsync(id, ct);

        if (flight is null)
        {
            return NotFound($"Flight with id {id} not found.");
        }

        return Ok(flight);
    }

    [HttpGet("{id:int}/details")]
    [ProducesResponseType(typeof(FlightResponseWithRelatedData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlightResponseWithRelatedData>> GetByIdWithRelatedData(int id, CancellationToken ct)
    {
        var flight = await _flightService.GetByIdWithRelatedDataAsync(id, ct);

        if (flight is null)
        {
            return NotFound($"Flight with id {id} not found.");
        }

        return Ok(flight);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<FlightListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FlightListItemResponse>>> Search(
        [FromQuery] FlightSearchQuery query,
        CancellationToken ct)
    {
        var flights = await _flightService.SearchAsync(query, ct);

        return Ok(flights);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FlightResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FlightResponseDto>> Create(
        [FromBody] CreateFlightRequest request,
        CancellationToken ct)
    {
        if (request.OriginAirportId == request.DestinationAirportId)
        {
            ModelState.AddModelError(nameof(request.DestinationAirportId), "Origin and Destination cannot be the same airport.");
            return ValidationProblem(ModelState);
        }

        var result = await _flightService.CreateAsync(request, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}