using AirportManagement.API.Controllers;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Booking;
using AirportManagement.Application.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace AirportManagement.Tests.Controllers;

public class BookingsControllerTests
{
    private readonly Mock<IBookingService> _service = new();
    private readonly BookingsController _controller;

    public BookingsControllerTests()
    {
        _controller = new BookingsController(_service.Object);
    }

    [Fact]
    public async Task Create_WhenValidRequest_Returns201CreatedWithConfirmation_AndCallsServiceOnce()
    {
        var ct = CancellationToken.None;

        var request = new CreateBookingRequest
        {
            FlightScheduleId = 123,
            FareClass = "Y",
            Currency = "EUR",
            BasePrice = 100m,
            Taxes = 20m,
            IsRefundable = false,
            Passengers =
            [
                new PassengerDto { FullName = "Jane Doe", Email = "jane@example.com", PhoneNumber = "0700000000" }
            ]
        };

        var response = new CreateBookingResponseDto
        {
            ConfirmationCode = "Q7H2K9",
            Status = "Active",
            TotalAmount = 120m,
            Currency = "EUR"
        };

        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct))
            .ReturnsAsync(Result<CreateBookingResponseDto>.Ok(response));

        var actionResult = await _controller.Create(request, ct);

        var created = actionResult.Result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.StatusCode.Should().Be(201);

        created.Value.Should().BeSameAs(response);
        response.ConfirmationCode.Should().NotBeNullOrWhiteSpace();

        created.RouteValues.Should().NotBeNull();
        created.RouteValues!["code"].Should().Be(response.ConfirmationCode);

        _service.Verify(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsNotFound_Returns404()
    {
        var ct = CancellationToken.None;
        var request = ValidCreateRequest();

        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct))
            .ReturnsAsync(Result<CreateBookingResponseDto>.Fail(ErrorType.NotFound, "Schedule not found."));

        var actionResult = await _controller.Create(request, ct);

        var notFound = actionResult.Result as NotFoundObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
        notFound.Value!.ToString().Should().Contain("Schedule not found");

        _service.Verify(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsConflict_Returns409()
    {
        var ct = CancellationToken.None;
        var request = ValidCreateRequest();

        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct))
            .ReturnsAsync(Result<CreateBookingResponseDto>.Fail(ErrorType.Conflict, "Overbooking."));

        var actionResult = await _controller.Create(request, ct);

        var conflict = actionResult.Result as ConflictObjectResult;
        conflict.Should().NotBeNull();
        conflict!.StatusCode.Should().Be(409);
        conflict.Value!.ToString().Should().Contain("Overbooking");

        _service.Verify(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsValidation_Returns400()
    {
        var ct = CancellationToken.None;
        var request = ValidCreateRequest();

        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct))
            .ReturnsAsync(Result<CreateBookingResponseDto>.Fail(ErrorType.Validation, "Bad request."));

        var actionResult = await _controller.Create(request, ct);

        var badRequest = actionResult.Result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value!.ToString().Should().Contain("Bad request");

        _service.Verify(s => s.CreateAsync(It.IsAny<CreateBookingRequest>(), ct), Times.Once);
    }

    [Fact]
    public async Task GetByCode_WhenFound_Returns200Ok_WithDto()
    {
        var ct = CancellationToken.None;
        const string code = "Q7H2K9";

        var dto = new BookingResponseDto
        {
            ConfirmationCode = code,
            Status = "Active",
            Quantity = 1,
            CreatedUtc = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc)
        };

        _service
            .Setup(s => s.GetByCodeAsync(code, ct))
            .ReturnsAsync(dto);

        var actionResult = await _controller.GetByCode(code, ct);

        var ok = actionResult.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var returned = ok.Value as BookingResponseDto;
        returned.Should().NotBeNull();
        returned!.ConfirmationCode.Should().Be(code);
        returned.Status.Should().Be("Active");
        returned.Quantity.Should().Be(1);
        returned.CreatedUtc.Should().Be(dto.CreatedUtc);

        _service.Verify(s => s.GetByCodeAsync(code, ct), Times.Once);
    }

    [Fact]
    public async Task GetByCode_WhenMissing_Returns404WithMessage()
    {
        var ct = CancellationToken.None;
        const string code = "MISSING";

        _service
            .Setup(s => s.GetByCodeAsync(code, ct))
            .ReturnsAsync((BookingResponseDto?)null);

        var actionResult = await _controller.GetByCode(code, ct);

        var notFound = actionResult.Result as NotFoundObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
        notFound.Value!.ToString().Should().Contain($"Booking '{code}' not found.");

        _service.Verify(s => s.GetByCodeAsync(code, ct), Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenSuccess_Returns204NoContent()
    {
        var ct = CancellationToken.None;
        const string code = "Q7H2K9";

        _service
            .Setup(s => s.CancelAsync(code, ct))
            .ReturnsAsync(Result.Ok());

        var result = await _controller.Cancel(code, ct);

        result.Should().BeOfType<NoContentResult>();

        _service.Verify(s => s.CancelAsync(code, ct), Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenNotFound_Returns404()
    {
        var ct = CancellationToken.None;
        const string code = "NOPE";

        _service
            .Setup(s => s.CancelAsync(code, ct))
            .ReturnsAsync(Result.Fail(ErrorType.NotFound, "Booking not found."));

        var result = await _controller.Cancel(code, ct);

        var notFound = result as NotFoundObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
        notFound.Value!.ToString().Should().Contain("Booking not found");

        _service.Verify(s => s.CancelAsync(code, ct), Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenConflict_Returns409()
    {
        var ct = CancellationToken.None;
        const string code = "Q7H2K9";

        _service
            .Setup(s => s.CancelAsync(code, ct))
            .ReturnsAsync(Result.Fail(ErrorType.Conflict, "Already cancelled."));

        var result = await _controller.Cancel(code, ct);

        var conflict = result as ConflictObjectResult;
        conflict.Should().NotBeNull();
        conflict!.StatusCode.Should().Be(409);
        conflict.Value!.ToString().Should().Contain("Already cancelled");

        _service.Verify(s => s.CancelAsync(code, ct), Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenValidation_Returns400()
    {
        var ct = CancellationToken.None;
        const string code = "Q7H2K9";

        _service
            .Setup(s => s.CancelAsync(code, ct))
            .ReturnsAsync(Result.Fail(ErrorType.Validation, "Bad request."));

        var result = await _controller.Cancel(code, ct);

        var bad = result as BadRequestObjectResult;
        bad.Should().NotBeNull();
        bad!.StatusCode.Should().Be(400);
        bad.Value!.ToString().Should().Contain("Bad request");

        _service.Verify(s => s.CancelAsync(code, ct), Times.Once);
    }



    private static CreateBookingRequest ValidCreateRequest()
        => new()
        {
            FlightScheduleId = 123,
            FareClass = "Y",
            Currency = "EUR",
            BasePrice = 100m,
            Taxes = 20m,
            IsRefundable = false,
            Passengers =
            [
                new PassengerDto { FullName = "Jane Doe", Email = "jane@example.com", PhoneNumber = "0700000000" }
            ]
        };
}
