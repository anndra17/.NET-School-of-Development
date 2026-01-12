using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Services;
using AirportManagement.Domain;
using AirportManagement.Domain.Models;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;

namespace AirportManagement.Tests.Services;

public class FlightScheduleServiceTests
{
    private readonly Mock<IFlightRepository> _flightsRepository = new();
    private readonly Mock<IGateRepository> _gatesRepository = new();
    private readonly Mock<IFlightScheduleRepository> _schedulesRepository = new();
    private readonly FlightScheduleService _service;
    private readonly Mock<IUnitOfWork> _uow = new();

    public FlightScheduleServiceTests()
    {
        _uow.SetupGet(x => x.FlightsRepository).Returns(_flightsRepository.Object);
        _uow.SetupGet(x => x.GatesRepository).Returns(_gatesRepository.Object);
        _uow.SetupGet(x => x.FlightSchedulesRepository).Returns(_schedulesRepository.Object);

        _service = new FlightScheduleService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenGateOverlapExists_ReturnsConflictAndDoesNotInsert()
    {
        // Arrange
        var ct = CancellationToken.None;

        var dto = new CreateScheduleRequestDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 01, 12, 11, 00, 00, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 01, 12, 13, 00, 00, DateTimeKind.Utc),
            GateCode = "A12",
            AssignedAircraftTail = null,
            Status = (int)FlightScheduleStatus.Planned
        };

        var flight = new Flight
        {
            Id = 1,
            OriginAirportId = 100,
            DestinationAirportId = 200
        };

        var gate = new Gate
        {
            Id = 55,
            AirportId = 100,
            Code = "A12"
        };

        _flightsRepository
            .Setup(r => r.GetByIdAsync(dto.FlightId, ct))
            .ReturnsAsync(flight);

        _gatesRepository
            .Setup(r => r.GetByAirportAndCodeAsync(flight.OriginAirportId, "A12", ct))
            .ReturnsAsync(gate);

        _schedulesRepository
           .Setup(r => r.ExistsGateOverlapAsync(
               gate.Id,
               dto.ScheduledDepartureUtc,
               dto.ScheduledArrivalUtc,
               null,
               ct))
           .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(dto, ct);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.ErrorMessage.Should().Contain("Gate overlap");
        result.Value.Should().BeNull();

        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        
        _schedulesRepository.Verify(r => r.ExistsGateOverlapAsync(
            gate.Id,
            dto.ScheduledDepartureUtc,
            dto.ScheduledArrivalUtc,
            null,
            ct), Times.Once);
    }
}