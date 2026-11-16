using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Flight;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Services;
using AirportManagement.Domain.Models;
using Moq;

namespace AirportManagement.Tests.Services;

public class FlightServiceTests
{
    private readonly Mock<IFlightRepository> _flightsRepo = new();
    private readonly Mock<IAirlineRepository> _airlinesRepo = new();
    private readonly Mock<IAirportRepository> _airportsRepo = new();
    private readonly Mock<IAircraftRepository> _aircraftsRepo = new();
    private readonly IFlightService _service;
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    public FlightServiceTests()
    {
        _unitOfWork.SetupGet(x => x.FlightsRepository).Returns(_flightsRepo.Object);
        _unitOfWork.SetupGet(x => x.AirlinesRepository).Returns(_airlinesRepo.Object);
        _unitOfWork.SetupGet(x => x.AirportsRepository).Returns(_airportsRepo.Object);
        _unitOfWork.SetupGet(x => x.AircraftsRepository).Returns(_aircraftsRepo.Object);

        _service = new FlightService(_unitOfWork.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityMissing_ReturnsNull()
    {
        var ct = CancellationToken.None;

        _flightsRepo
            .Setup(r => r.GetByIdAsync(123, ct))
            .ReturnsAsync((Flight?)null);

        var result = await _service.GetByIdAsync(123, ct);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsMappedDto()
    {
        var ct = CancellationToken.None;

        var entity = CreateFlightEntity(
            id: 1,
            airlineId: 10,
            flightNumber: "RO1234",
            originAirportId: 20,
            destinationAirportId: 30,
            defaultAircraftId: null,
            isActive: true);

        _flightsRepo
            .Setup(r => r.GetByIdAsync(1, ct))
            .ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(1, ct);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal(10, result.AirlineId);
        Assert.Equal("RO1234", result.FlightNumber);
        Assert.Equal(20, result.OriginAirportId);
        Assert.Equal(30, result.DestinationAirportId);
        Assert.Null(result.DefaultAircraftId);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdWithRelatedDataAsync_ReturnsRepositoryResult()
    {
        var ct = CancellationToken.None;

        var dto = new FlightResponseWithRelatedData
        {
            Id = 5,
            AirlineId = 1,
            FlightNumber = "RO0001",
            OriginAirportId = 2,
            DestinationAirportId = 3,
            DefaultAircraftId = 9,
            IsActive = true,
            AirlineIataCode = "RO",
            AirlineName = "Test Airline",
            OriginIataCode = "OTP",
            DestinationIataCode = "CLJ",
            DefaultAircraftTailNumber = "YR-TST"
        };

        _flightsRepo
            .Setup(r => r.GetByIdWithRelatedDataAsync(5, ct))
            .ReturnsAsync(dto);

        var result = await _service.GetByIdWithRelatedDataAsync(5, ct);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("RO0001", result.FlightNumber);
        Assert.Equal(1, result.AirlineId);
    }

    [Fact]
    public async Task CreateAsync_WhenOriginEqualsDestination_ReturnsValidationFail_AndDoesNotWrite()
    {
        var ct = CancellationToken.None;

        var req = new CreateFlightRequest
        {
            AirlineId = 10,
            FlightNumber = "RO1234",
            OriginAirportId = 5,
            DestinationAirportId = 5,
            DefaultAircraftId = null,
            IsActive = true
        };

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotNull(result.ErrorMessage);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenAirlineNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(false);

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenOriginAirportNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(false);

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDestinationAirportNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.DestinationAirportId, ct)).ReturnsAsync(false);

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDefaultAircraftProvidedButMissing_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();
        req.DefaultAircraftId = 77;

        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.DestinationAirportId, ct)).ReturnsAsync(true);
        _aircraftsRepo.Setup(r => r.ExistsAsync(req.DefaultAircraftId.Value, ct)).ReturnsAsync(false);

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenConflict_ReturnsConflictFail()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        SetupAllExistsForCreate(req, ct, aircraftExists: true);

        _flightsRepo
            .Setup(r => r.ExistsByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync(true);

        var result = await _service.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_Inserts_Saves_AndReturnsOk_FromRepoFetch()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        SetupAllExistsForCreate(req, ct, aircraftExists: true);

        _flightsRepo
            .Setup(r => r.ExistsByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync(false);

        var createdEntity = CreateFlightEntity(
            id: 99,
            airlineId: req.AirlineId,
            flightNumber: req.FlightNumber,
            originAirportId: req.OriginAirportId,
            destinationAirportId: req.DestinationAirportId,
            defaultAircraftId: req.DefaultAircraftId,
            isActive: req.IsActive ?? true);

        _flightsRepo
            .Setup(r => r.GetByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync(createdEntity);

        var result = await _service.CreateAsync(req, ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(99, result.Value!.Id);
        Assert.Equal(req.AirlineId, result.Value.AirlineId);
        Assert.Equal(req.FlightNumber, result.Value.FlightNumber);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Once);
        _flightsRepo.Verify(r => r.GetByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_AndRepoFetchReturnsNull_ReturnsOk_FallbackMapping()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        SetupAllExistsForCreate(req, ct, aircraftExists: true);

        _flightsRepo
            .Setup(r => r.ExistsByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync(false);

        _flightsRepo
            .Setup(r => r.GetByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync((Flight?)null);

        var result = await _service.CreateAsync(req, ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(req.AirlineId, result.Value!.AirlineId);
        Assert.Equal(req.FlightNumber, result.Value.FlightNumber);

        _flightsRepo.Verify(r => r.InsertAsync(It.IsAny<Flight>(), ct), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenFlightNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidUpdateRequest();

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync((Flight?)null);

        var result = await _service.UpdateAsync(1, req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.UpdateAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenAirlineNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;
        var req = ValidUpdateRequest();

        var entity = CreateFlightEntity(
            id: 1, airlineId: 10, flightNumber: "RO1111",
            originAirportId: 20, destinationAirportId: 30,
            defaultAircraftId: null, isActive: true);

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(false);

        var result = await _service.UpdateAsync(1, req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.UpdateAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenConflict_ReturnsConflictFail()
    {
        var ct = CancellationToken.None;
        var req = ValidUpdateRequest();

        var entity = CreateFlightEntity(
            id: 1,
            airlineId: 10,
            flightNumber: "RO1111",
            originAirportId: 20,
            destinationAirportId: 30,
            defaultAircraftId: null,
            isActive: true);

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.DestinationAirportId, ct)).ReturnsAsync(true);

        if (req.DefaultAircraftId is not null)
            _aircraftsRepo.Setup(r => r.ExistsAsync(req.DefaultAircraftId.Value, ct)).ReturnsAsync(true);

        _flightsRepo
            .Setup(r => r.ExistsByAirlineAndNumberExceptAsync(req.AirlineId, req.FlightNumber, 1, ct))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(1, req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        _flightsRepo.Verify(r => r.UpdateAsync(It.IsAny<Flight>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenValid_Updates_Saves_AndReturnsOk()
    {
        var ct = CancellationToken.None;
        var req = ValidUpdateRequest();

        var entity = CreateFlightEntity(
            id: 1,
            airlineId: 10,
            flightNumber: "RO1111",
            originAirportId: 20,
            destinationAirportId: 30,
            defaultAircraftId: null,
            isActive: true);

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.DestinationAirportId, ct)).ReturnsAsync(true);

        if (req.DefaultAircraftId is not null)
            _aircraftsRepo.Setup(r => r.ExistsAsync(req.DefaultAircraftId.Value, ct)).ReturnsAsync(true);

        _flightsRepo
            .Setup(r => r.ExistsByAirlineAndNumberExceptAsync(req.AirlineId, req.FlightNumber, 1, ct))
            .ReturnsAsync(false);

        var updatedEntity = CreateFlightEntity(
            id: 1,
            airlineId: req.AirlineId,
            flightNumber: req.FlightNumber,
            originAirportId: req.OriginAirportId,
            destinationAirportId: req.DestinationAirportId,
            defaultAircraftId: req.DefaultAircraftId,
            isActive: req.IsActive);

        _flightsRepo
            .Setup(r => r.GetByAirlineAndNumberAsync(req.AirlineId, req.FlightNumber, ct))
            .ReturnsAsync(updatedEntity);

        var result = await _service.UpdateAsync(1, req, ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal(req.AirlineId, result.Value.AirlineId);
        Assert.Equal(req.FlightNumber, result.Value.FlightNumber);
        Assert.Equal(req.IsActive, result.Value.IsActive);

        _flightsRepo.Verify(r => r.UpdateAsync(It.IsAny<Flight>(), ct), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenFlightNotFound_ReturnsNotFoundFail()
    {
        var ct = CancellationToken.None;

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync((Flight?)null);

        var result = await _service.DeleteAsync(1, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _flightsRepo.Verify(r => r.DeleteAsync(It.IsAny<int>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenHasSchedules_ReturnsConflictFail()
    {
        var ct = CancellationToken.None;

        var entity = CreateFlightEntity(
            id: 1,
            airlineId: 10,
            flightNumber: "RO1234",
            originAirportId: 20,
            destinationAirportId: 30,
            defaultAircraftId: null,
            isActive: true);

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
        _flightsRepo.Setup(r => r.HasSchedulesAsync(1, ct)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        _flightsRepo.Verify(r => r.DeleteAsync(It.IsAny<int>(), ct), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNoDependencies_Deletes_AndSaves_ReturnsOk()
    {
        var ct = CancellationToken.None;

        var entity = CreateFlightEntity(
            id: 1,
            airlineId: 10,
            flightNumber: "RO1234",
            originAirportId: 20,
            destinationAirportId: 30,
            defaultAircraftId: null,
            isActive: true);

        _flightsRepo.Setup(r => r.GetByIdAsync(1, ct)).ReturnsAsync(entity);
        _flightsRepo.Setup(r => r.HasSchedulesAsync(1, ct)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(1, ct);

        Assert.True(result.Success);

        _flightsRepo.Verify(r => r.DeleteAsync(1, ct), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }

    private static CreateFlightRequest ValidCreateRequest()
            => new()
            {
                AirlineId = 10,
                FlightNumber = "RO1234",
                OriginAirportId = 20,
                DestinationAirportId = 30,
                DefaultAircraftId = null,
                IsActive = true
            };

    private static UpdateFlightRequest ValidUpdateRequest()
        => new()
        {
            AirlineId = 10,
            FlightNumber = "RO5678",
            OriginAirportId = 20,
            DestinationAirportId = 30,
            DefaultAircraftId = null,
            IsActive = true
        };

    private void SetupAllExistsForCreate(CreateFlightRequest req, CancellationToken ct, bool aircraftExists)
    {
        _airlinesRepo.Setup(r => r.ExistsAsync(req.AirlineId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.OriginAirportId, ct)).ReturnsAsync(true);
        _airportsRepo.Setup(r => r.ExistsAsync(req.DestinationAirportId, ct)).ReturnsAsync(true);

        if (req.DefaultAircraftId is not null)
            _aircraftsRepo.Setup(r => r.ExistsAsync(req.DefaultAircraftId.Value, ct)).ReturnsAsync(aircraftExists);
    }

    private static Flight CreateFlightEntity(
        int id,
        int airlineId,
        string flightNumber,
        int originAirportId,
        int destinationAirportId,
        int? defaultAircraftId,
        bool isActive)
    {
        return new Flight
        {
            Id = id,
            AirlineId = airlineId,
            FlightNumber = flightNumber,
            OriginAirportId = originAirportId,
            DestinationAirportId = destinationAirportId,
            DefaultAircraftId = defaultAircraftId,
            IsActive = isActive
        };
    }
}

