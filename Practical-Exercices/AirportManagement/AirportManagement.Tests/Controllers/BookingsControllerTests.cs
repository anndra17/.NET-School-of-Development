using AirportManagement.API.Controllers;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Booking;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;


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
}
