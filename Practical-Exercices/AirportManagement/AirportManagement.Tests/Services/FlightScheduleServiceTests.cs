using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Dtos.Schedule;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Services;
using AirportManagement.Domain;
using AirportManagement.Domain.Models;
using FluentAssertions;
using Moq;
using System.Text;
using System.Text.Json;

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
    public async Task GetByIdAsync_WhenScheduleNotFound_ReturnsNull()
    {
        var ct = CancellationToken.None;

        _schedulesRepository
            .Setup(r => r.GetByIdAsync(123, ct))
            .ReturnsAsync((FlightSchedule?)null);

        var result = await _service.GetByIdAsync(123, ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenScheduleFound_ReturnsMappedDto()
    {
        var ct = CancellationToken.None;

        var schedule = new FlightSchedule
        {
            Id = 10,
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 12, 0, 0, DateTimeKind.Utc),
            GateId = 55,
            AssignedAircraftId = null,
            Status = FlightScheduleStatus.Planned
        };

        _schedulesRepository
            .Setup(r => r.GetByIdAsync(10, ct))
            .ReturnsAsync(schedule);

        var result = await _service.GetByIdAsync(10, ct);

        result.Should().NotBeNull();
        result!.FlightId.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WhenFlightNotFound_ReturnsNotFound()
    {
        var ct = CancellationToken.None;

        var dto = new CreateScheduleRequestDto
        {
            FlightId = 999,
            ScheduledDepartureUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 13, 0, 0, DateTimeKind.Utc),
            GateCode = "A12"
        };

        _flightsRepository
            .Setup(r => r.GetByIdAsync(dto.FlightId, ct))
            .ReturnsAsync((Flight?)null);

        var result = await _service.CreateAsync(dto, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Contain("Flight");
        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenGateNotFound_ReturnsNotFound()
    {
        var ct = CancellationToken.None;

        var dto = new CreateScheduleRequestDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 13, 0, 0, DateTimeKind.Utc),
            GateCode = "A12"
        };

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };

        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);
        _gatesRepository.Setup(r => r.GetByAirportAndCodeAsync(100, "A12", ct)).ReturnsAsync((Gate?)null);

        var result = await _service.CreateAsync(dto, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Contain("Gate not found");
        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenGateOverlapExists_ReturnsConflictAndDoesNotInsert()
    {
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

        var result = await _service.CreateAsync(dto, ct);

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

    [Fact]
    public async Task CreateAsync_WhenCreatedCannotBeReloaded_ReturnsConflict()
    {
        var ct = CancellationToken.None;

        var dto = new CreateScheduleRequestDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 01, 12, 11, 00, 00, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 01, 12, 13, 00, 00, DateTimeKind.Utc),
            GateCode = null
        };

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };

        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);

        _schedulesRepository.Setup(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        _schedulesRepository
            .Setup(r => r.GetByFlightIdAndDepartureAsync(1, dto.ScheduledDepartureUtc, ct))
            .ReturnsAsync((FlightSchedule?)null);

        var result = await _service.CreateAsync(dto, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.ErrorMessage.Should().Contain("could not be reloaded");

        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenValidAndNoOverlap_ReturnsOkAndInsertsAndSaves()
    {
        var ct = CancellationToken.None;

        var dto = new CreateScheduleRequestDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 01, 12, 11, 00, 00, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 01, 12, 13, 00, 00, DateTimeKind.Utc),
            GateCode = "A12"
        };

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };
        var gate = new Gate { Id = 55, AirportId = 100, Code = "A12" };

        var createdSchedule = new FlightSchedule
        {
            Id = 777,
            FlightId = 1,
            ScheduledDepartureUtc = dto.ScheduledDepartureUtc,
            ScheduledArrivalUtc = dto.ScheduledArrivalUtc,
            GateId = 55,
            Status = FlightScheduleStatus.Planned
        };

        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);
        _gatesRepository.Setup(r => r.GetByAirportAndCodeAsync(100, "A12", ct)).ReturnsAsync(gate);

        _schedulesRepository
            .Setup(r => r.ExistsGateOverlapAsync(55, dto.ScheduledDepartureUtc, dto.ScheduledArrivalUtc, null, ct))
            .ReturnsAsync(false);

        _schedulesRepository.Setup(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        _schedulesRepository
            .Setup(r => r.GetByFlightIdAndDepartureAsync(1, dto.ScheduledDepartureUtc, ct))
            .ReturnsAsync(createdSchedule);

        var result = await _service.CreateAsync(dto, ct);

        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FlightId.Should().Be(1);

        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenJsonInvalid_ReturnsValidationFail()
    {
        var ct = CancellationToken.None;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{ NOT JSON"));

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);
        result.ErrorMessage.Should().Contain("Invalid JSON");
    }

    [Fact]
    public async Task ImportAsync_WhenJsonHasNoRows_ReturnsValidationFail()
    {
        var ct = CancellationToken.None;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[]"));

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);
        result.ErrorMessage.Should().Contain("contains no rows");
    }

    [Fact]
    public async Task ImportAsync_WhenRowHasInvalidFlightId_AddsErrorAndSkipsRow()
    {
        var ct = CancellationToken.None;

        var rows = new List<ImportScheduleRowDto>
        {
            new()
            {
                FlightId = 0,
                ScheduledDepartureUtc = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc   = new DateTime(2026, 1, 12, 12, 0, 0, DateTimeKind.Utc),
                Status = 0
            }
        };

        using var stream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(rows));

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Total.Should().Be(1);
        result.Value.Created.Should().Be(0);
        result.Value.Updated.Should().Be(0);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors[0].Message.Should().Contain("flightId is required");

        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_WhenGateOverlap_AddsErrorAndDoesNotInsert()
    {
        var ct = CancellationToken.None;

        var rows = new List<ImportScheduleRowDto>
        {
            new()
            {
                FlightId = 1,
                ScheduledDepartureUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc   = new DateTime(2026, 1, 12, 13, 0, 0, DateTimeKind.Utc),
                GateCode = "A12",
                Status = 0
            }
        };

        using var stream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(rows));

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };
        var gate = new Gate { Id = 55, AirportId = 100, Code = "A12" };

        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);
        _gatesRepository.Setup(r => r.GetByAirportAndCodeAsync(100, "A12", ct)).ReturnsAsync(gate);

        _schedulesRepository
            .Setup(r => r.GetByFlightIdAndDepartureAsync(1, rows[0].ScheduledDepartureUtc, ct))
            .ReturnsAsync((FlightSchedule?)null);

        _schedulesRepository
            .Setup(r => r.ExistsGateOverlapAsync(55, rows[0].ScheduledDepartureUtc, rows[0].ScheduledArrivalUtc, null, ct))
            .ReturnsAsync(true);

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeTrue();
        result.Value!.Created.Should().Be(0);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors[0].Message.Should().Contain("Gate overlap");

        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenNewScheduleNoOverlap_InsertsAndCountsCreated()
    {
        var ct = CancellationToken.None;

        var row = new ImportScheduleRowDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 13, 0, 0, DateTimeKind.Utc),
            GateCode = "A12",
            Status = 0
        };

        using var stream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(new List<ImportScheduleRowDto> { row }));

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };
        var gate = new Gate { Id = 55, AirportId = 100, Code = "A12" };

        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);
        _gatesRepository.Setup(r => r.GetByAirportAndCodeAsync(100, "A12", ct)).ReturnsAsync(gate);

        _schedulesRepository
            .Setup(r => r.GetByFlightIdAndDepartureAsync(1, row.ScheduledDepartureUtc, ct))
            .ReturnsAsync((FlightSchedule?)null);

        _schedulesRepository
            .Setup(r => r.ExistsGateOverlapAsync(55, row.ScheduledDepartureUtc, row.ScheduledArrivalUtc, null, ct))
            .ReturnsAsync(false);

        _schedulesRepository.Setup(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeTrue();
        result.Value!.Total.Should().Be(1);
        result.Value.Created.Should().Be(1);
        result.Value.Updated.Should().Be(0);
        result.Value.Errors.Should().BeEmpty();

        _schedulesRepository.Verify(r => r.InsertAsync(It.IsAny<FlightSchedule>(), ct), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_WhenExistingSchedule_UpdatesAndCountsUpdated()
    {
        var ct = CancellationToken.None;

        var row = new ImportScheduleRowDto
        {
            FlightId = 1,
            ScheduledDepartureUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 13, 0, 0, DateTimeKind.Utc),
            GateCode = null,
            Status = 0
        };

        using var stream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(new List<ImportScheduleRowDto> { row }));

        var flight = new Flight { Id = 1, OriginAirportId = 100, DestinationAirportId = 200 };
        _flightsRepository.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(flight);

        var existing = new FlightSchedule
        {
            Id = 999,
            FlightId = 1,
            ScheduledDepartureUtc = row.ScheduledDepartureUtc,
            ScheduledArrivalUtc = new DateTime(2026, 1, 12, 12, 0, 0, DateTimeKind.Utc),
            GateId = null,
            Status = FlightScheduleStatus.Planned
        };

        _schedulesRepository
            .Setup(r => r.GetByFlightIdAndDepartureAsync(1, row.ScheduledDepartureUtc, ct))
            .ReturnsAsync(existing);

        _schedulesRepository
            .Setup(r => r.ExistsGateOverlapAsync(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _schedulesRepository.Setup(r => r.UpdateAsync(existing, ct)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        var result = await _service.ImportAsync(stream, ct);

        result.Success.Should().BeTrue();
        result.Value!.Updated.Should().Be(1);
        result.Value.Created.Should().Be(0);

        _schedulesRepository.Verify(r => r.UpdateAsync(existing, ct), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenPageAndPageSizeInvalid_NormalizesAndCallsRepositoryWithDefaults()
    {
        var ct = CancellationToken.None;

        var query = new ScheduleSearchQuery
        {
            Origin = "OTP",
            Destination = "LHR",
            Date = new DateOnly(2026, 1, 12),
            Page = 0,       
            PageSize = 0    
        };

        var items = new List<ScheduleListItemResponse>();
        _schedulesRepository
            .Setup(r => r.SearchAsync(query.Origin, query.Destination, query.Date, 1, 20, ct))
            .ReturnsAsync((items, 0));

        var result = await _service.SearchAsync(query, ct);

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task SearchAsync_WhenPageSizeTooLarge_ClampsTo100()
    {
        var ct = CancellationToken.None;

        var query = new ScheduleSearchQuery
        {
            Origin = null,
            Destination = null,
            Date = null,
            Page = 2,
            PageSize = 500 
        };

        var items = new List<ScheduleListItemResponse> { new() };
        _schedulesRepository
            .Setup(r => r.SearchAsync(null, null, null, 2, 100, ct))
            .ReturnsAsync((items, 101));

        var result = await _service.SearchAsync(query, ct);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(100);
        result.TotalItems.Should().Be(101);
        result.TotalPages.Should().Be(2); 
    }

    [Fact]
    public async Task DeleteAsync_WhenScheduleNotFound_ReturnsNotFound()
    {
        var ct = CancellationToken.None;

        _schedulesRepository.Setup(r => r.GetByIdAsync(10, ct)).ReturnsAsync((FlightSchedule?)null);

        var result = await _service.DeleteAsync(10, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);

        _schedulesRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenHasTickets_ReturnsConflict()
    {
        var ct = CancellationToken.None;

        var schedule = new FlightSchedule { Id = 10, FlightId = 1 };
        _schedulesRepository.Setup(r => r.GetByIdAsync(10, ct)).ReturnsAsync(schedule);
        _schedulesRepository.Setup(r => r.HasTicketsAsync(10, ct)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(10, ct);

        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.ErrorMessage.Should().Contain("tickets");

        _schedulesRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNoTickets_DeletesAndSaves_ReturnsOk()
    {
        var ct = CancellationToken.None;

        var schedule = new FlightSchedule { Id = 10, FlightId = 1 };
        _schedulesRepository.Setup(r => r.GetByIdAsync(10, ct)).ReturnsAsync(schedule);
        _schedulesRepository.Setup(r => r.HasTicketsAsync(10, ct)).ReturnsAsync(false);

        _schedulesRepository.Setup(r => r.DeleteAsync(10, ct)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        var result = await _service.DeleteAsync(10, ct);

        result.Success.Should().BeTrue();

        _schedulesRepository.Verify(r => r.DeleteAsync(10, ct), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }
}