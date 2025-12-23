using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Application.Enums;
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

    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportSchedulesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportSchedulesResponseDto>> Import(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "File is required." });

        if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Only .json files are supported." });

        await using var stream = file.OpenReadStream();
        var result = await _scheduleService.ImportAsync(stream, ct);

        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ScheduleListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ScheduleListItemResponse>>> Search(
    [FromQuery] ScheduleSearchQuery query,
    CancellationToken ct)
    {
        var result = await _scheduleService.SearchAsync(query, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _scheduleService.DeleteAsync(id, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return NoContent();
    }
}
