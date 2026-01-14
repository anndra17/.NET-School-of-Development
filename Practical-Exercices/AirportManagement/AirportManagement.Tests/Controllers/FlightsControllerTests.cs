using AirportManagement.API.Controllers;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirportManagement.Tests.Controllers;

public class FlightsControllerTests
{
    private readonly Mock<IFlightService> _flightService = new();
    private readonly FlightsController _sut;

    public FlightsControllerTests()
    {
        _sut = new FlightsController(_flightService.Object);
    }

    [Fact]
    public async Task GetById_WhenFlightExists_ReturnsOkObjectResult_WithDto()
    {
        var ct = CancellationToken.None;

        var dto = new FlightResponseDto
        {
            Id = 1,
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

        _flightService
            .Setup(s => s.GetByIdAsync(1, ct))
            .ReturnsAsync(dto);

        var result = await _sut.GetById(1, ct);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<FlightResponseDto>(ok.Value);

        Assert.Equal(1, value.Id);
        Assert.Equal("RO1234", value.FlightNumber);

        _flightService.Verify(s => s.GetByIdAsync(1, ct), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenFlightMissing_ReturnsNotFoundObjectResult()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.GetByIdAsync(99, ct))
            .ReturnsAsync((FlightResponseDto?)null);

        var result = await _sut.GetById(99, ct);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
        Assert.Contains("Flight with id 99 not found.", notFound.Value?.ToString());

        _flightService.Verify(s => s.GetByIdAsync(99, ct), Times.Once);
    }

    [Fact]
    public async Task GetByIdWithRelatedData_WhenFlightExists_ReturnsOkObjectResult_WithDto()
    {
        var ct = CancellationToken.None;

        var dto = new FlightResponseWithRelatedData
        {
            Id = 2,
            AirlineId = 11,
            FlightNumber = "RO5678",
            OriginAirportId = 21,
            DestinationAirportId = 31,
            DefaultAircraftId = 100,
            IsActive = true,
            AirlineIataCode = "RO",
            AirlineName = "Test Airline",
            OriginIataCode = "OTP",
            DestinationIataCode = "CLJ",
            DefaultAircraftTailNumber = "YR-TST"
        };

        _flightService
            .Setup(s => s.GetByIdWithRelatedDataAsync(2, ct))
            .ReturnsAsync(dto);

        var result = await _sut.GetByIdWithRelatedData(2, ct);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<FlightResponseWithRelatedData>(ok.Value);

        Assert.Equal(2, value.Id);
        Assert.Equal("RO", value.AirlineIataCode);

        _flightService.Verify(s => s.GetByIdWithRelatedDataAsync(2, ct), Times.Once);
    }

    [Fact]
    public async Task GetByIdWithRelatedData_WhenFlightMissing_ReturnsNotFoundObjectResult()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.GetByIdWithRelatedDataAsync(77, ct))
            .ReturnsAsync((FlightResponseWithRelatedData?)null);

        var result = await _sut.GetByIdWithRelatedData(77, ct);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
        Assert.Contains("Flight with id 77 not found.", notFound.Value?.ToString());

        _flightService.Verify(s => s.GetByIdWithRelatedDataAsync(77, ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsSuccess_ReturnsCreatedAtAction_WithDto()
    {
        var ct = CancellationToken.None;

        var req = new CreateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

        var created = new FlightResponseDto
        {
            Id = 123,
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

        _flightService
            .Setup(s => s.CreateAsync(req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Ok(created));

        var result = await _sut.Create(req, ct);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(FlightsController.GetById), createdAt.ActionName);

        var value = Assert.IsType<FlightResponseDto>(createdAt.Value);
        Assert.Equal(123, value.Id);

        _flightService.Verify(s => s.CreateAsync(req, ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsNotFound_ReturnsNotFoundObjectResult_WithMessage()
    {
        var ct = CancellationToken.None;

        var req = new CreateFlightRequest
        {
            AirlineId = 999,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 30
        };

        _flightService
            .Setup(s => s.CreateAsync(req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.NotFound, "Airline 999 not found."));

        var result = await _sut.Create(req, ct);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
        Assert.NotNull(notFound.Value);
        Assert.Contains("Airline 999 not found", notFound.Value!.ToString());

        _flightService.Verify(s => s.CreateAsync(req, ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsConflict_ReturnsConflictObjectResult_WithMessage()
    {
        var ct = CancellationToken.None;

        var req = new CreateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 30
        };

        _flightService
            .Setup(s => s.CreateAsync(req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.Conflict, "Already exists."));

        var result = await _sut.Create(req, ct);

        var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflict.StatusCode);
        Assert.Contains("Already exists", conflict.Value?.ToString());

        _flightService.Verify(s => s.CreateAsync(req, ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsValidation_ReturnsBadRequestObjectResult_WithMessage()
    {
        var ct = CancellationToken.None;

        var req = new CreateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 20,
            DestinationAirportId = 20
        };

        _flightService
            .Setup(s => s.CreateAsync(req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.Validation, "Origin and Destination cannot be the same airport."));

        var result = await _sut.Create(req, ct);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Contains("Origin and Destination", badRequest.Value?.ToString());

        _flightService.Verify(s => s.CreateAsync(req, ct), Times.Once);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsSuccess_ReturnsOkObjectResult_WithDto()
    {
        var ct = CancellationToken.None;

        var req = new UpdateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO9999",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

        var updated = new FlightResponseDto
        {
            Id = 1,
            AirlineId = 10,
            FlightNumber = "RO9999",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

        _flightService
            .Setup(s => s.UpdateAsync(1, req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Ok(updated));

        var result = await _sut.Update(1, req, ct);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<FlightResponseDto>(ok.Value);
        Assert.Equal("RO9999", value.FlightNumber);

        _flightService.Verify(s => s.UpdateAsync(1, req, ct), Times.Once);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsNotFound_ReturnsNotFoundObjectResult()
    {
        var ct = CancellationToken.None;

        var req = new UpdateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO9999",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            IsActive = true
        };

        _flightService
            .Setup(s => s.UpdateAsync(1, req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.NotFound, "Flight 1 not found."));

        var result = await _sut.Update(1, req, ct);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
        Assert.Contains("not found", notFound.Value?.ToString());

        _flightService.Verify(s => s.UpdateAsync(1, req, ct), Times.Once);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsConflict_ReturnsConflictObjectResult()
    {
        var ct = CancellationToken.None;

        var req = new UpdateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO9999",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            IsActive = true
        };

        _flightService
            .Setup(s => s.UpdateAsync(1, req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.Conflict, "Conflict"));

        var result = await _sut.Update(1, req, ct);

        var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflict.StatusCode);

        _flightService.Verify(s => s.UpdateAsync(1, req, ct), Times.Once);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsValidation_ReturnsBadRequestObjectResult()
    {
        var ct = CancellationToken.None;

        var req = new UpdateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO9999",
            OriginAirportId = 20,
            DestinationAirportId = 20,
            IsActive = true
        };

        _flightService
            .Setup(s => s.UpdateAsync(1, req, ct))
            .ReturnsAsync(Result<FlightResponseDto>.Fail(ErrorType.Validation, "Validation error"));

        var result = await _sut.Update(1, req, ct);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequest.StatusCode);

        _flightService.Verify(s => s.UpdateAsync(1, req, ct), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsSuccess_ReturnsNoContent()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.DeleteAsync(1, ct))
            .ReturnsAsync(Result.Ok());

        var result = await _sut.Delete(1, ct);

        Assert.IsType<NoContentResult>(result);

        _flightService.Verify(s => s.DeleteAsync(1, ct), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsNotFound_ReturnsNotFoundObjectResult()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.DeleteAsync(1, ct))
            .ReturnsAsync(Result.Fail(ErrorType.NotFound, "Flight 1 not found."));

        var result = await _sut.Delete(1, ct);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
        Assert.Contains("not found", notFound.Value?.ToString());

        _flightService.Verify(s => s.DeleteAsync(1, ct), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsConflict_ReturnsConflictObjectResult()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.DeleteAsync(1, ct))
            .ReturnsAsync(Result.Fail(ErrorType.Conflict, "Cannot delete"));

        var result = await _sut.Delete(1, ct);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflict.StatusCode);
        Assert.Contains("Cannot delete", conflict.Value?.ToString());

        _flightService.Verify(s => s.DeleteAsync(1, ct), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsValidationOrOtherError_ReturnsBadRequestObjectResult()
    {
        var ct = CancellationToken.None;

        _flightService
            .Setup(s => s.DeleteAsync(1, ct))
            .ReturnsAsync(Result.Fail(ErrorType.Validation, "Bad request"));

        var result = await _sut.Delete(1, ct);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, bad.StatusCode);

        _flightService.Verify(s => s.DeleteAsync(1, ct), Times.Once);
    }
}
