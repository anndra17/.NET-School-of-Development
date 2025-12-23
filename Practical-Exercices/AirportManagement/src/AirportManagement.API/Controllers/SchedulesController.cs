using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Schedule;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IFlightScheduleService _scheduleService;

    public SchedulesController(IFlightScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ScheduleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleResponseDto>> GetById(int id, CancellationToken ct)
    {
        var scheduleDto = await _scheduleService.GetByIdAsync(id, ct);

        if (scheduleDto is null)
        {
            return NotFound(new { message = $"FlightSchedule {id} not found." });
        }

        return Ok(scheduleDto);
    }
}
