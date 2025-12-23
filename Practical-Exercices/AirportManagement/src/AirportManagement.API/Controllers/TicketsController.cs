using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Ticket;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets)
    {
        _tickets = tickets;
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> GetById(long id, CancellationToken ct)
    {
        var dto = await _tickets.GetByIdAsync(id, ct);
        return dto is null ? NotFound(new { message = $"Ticket {id} not found." }) : Ok(dto);
    }

    [HttpGet("availability")]
    [ProducesResponseType(typeof(TicketAvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketAvailabilityResponseDto>> Availability([FromQuery] int flightScheduleId, CancellationToken ct)
    {
        var result = await _tickets.GetAvailabilityAsync(flightScheduleId, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> Create([FromBody] CreateTicketRequest request, CancellationToken ct)
    {
        var result = await _tickets.CreateAsync(request, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _tickets.DeleteAsync(id, ct);

        if (!result.Success)
            return NotFound(new { message = result.ErrorMessage });

        return NoContent();
    }

    [HttpGet("by-schedule/{flightScheduleId:int}")]
    [ProducesResponseType(typeof(IEnumerable<TicketResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetBySchedule(
    int flightScheduleId,
    CancellationToken ct)
    {
        var tickets = await _tickets.GetByScheduleAsync(flightScheduleId, ct);

        return Ok(tickets);
    }

    [HttpPut("{id:long}/inventory")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> UpdateInventory(
        long id,
        [FromBody] UpdateTicketInventoryRequest request,
        CancellationToken ct)
    {
        var result = await _tickets.UpdateInventoryAsync(id, request, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return Ok(result.Value);
    }
}
