using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Booking;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;

    public BookingsController(IBookingService bookings)
    {
        _bookings = bookings;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBookingResponseDto>> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var result = await _bookings.CreateAsync(request, ct);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return CreatedAtAction(nameof(GetByCode), new { code = result.Value!.ConfirmationCode }, result.Value);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponseDto>> GetByCode(string code, CancellationToken ct)
    {
        var dto = await _bookings.GetByCodeAsync(code, ct);
        if (dto is null)
            return NotFound(new { message = $"Booking '{code}' not found." });

        return Ok(dto);
    }

    [HttpDelete("{code}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(string code, CancellationToken ct)
    {
        var result = await _bookings.CancelAsync(code, ct);

        if (!result.Success)
            return NotFound(new { message = result.ErrorMessage });

        return NoContent();
    }
}
