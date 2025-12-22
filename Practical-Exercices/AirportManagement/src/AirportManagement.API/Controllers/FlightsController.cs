using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Domain.Models;
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
            return NotFound();
        }

        return Ok(flight);
    }
}
