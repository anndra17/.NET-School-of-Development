using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Dtos.Booking;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Services;
using AirportManagement.Domain.Enums;
using AirportManagement.Domain.Models;
using Moq;

namespace AirportManagement.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();

    private readonly Mock<IBookingRepository> _bookingsRepo = new();
    private readonly Mock<IFlightScheduleRepository> _schedulesRepo = new();
    private readonly Mock<ITicketRepository> _ticketsRepo = new();
    private readonly Mock<IFlightRepository> _flightsRepo = new();
    private readonly Mock<IAircraftRepository> _aircraftsRepo = new();

    private readonly IBookingService _sut;

    public BookingServiceTests()
    {
        _uow.SetupGet(x => x.BookingsRepository).Returns(_bookingsRepo.Object);
        _uow.SetupGet(x => x.FlightSchedulesRepository).Returns(_schedulesRepo.Object);
        _uow.SetupGet(x => x.TicketsRepository).Returns(_ticketsRepo.Object);
        _uow.SetupGet(x => x.FlightsRepository).Returns(_flightsRepo.Object);
        _uow.SetupGet(x => x.AircraftsRepository).Returns(_aircraftsRepo.Object);


        _uow
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((action, ct) => action(ct));

        _sut = new BookingService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenPassengersNullOrEmpty_ReturnsValidation()
    {
        var ct = CancellationToken.None;

        var req = ValidCreateRequest();
        req = new CreateBookingRequest
        {
            FlightScheduleId = req.FlightScheduleId,
            FareClass = req.FareClass,
            BasePrice = req.BasePrice,
            Taxes = req.Taxes,
            Currency = req.Currency,
            IsRefundable = req.IsRefundable,
            Passengers = new List<PassengerDto>() 
        };

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Validation, result.ErrorType);

        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenFareClassInvalid_ReturnsValidation()
    {
        var ct = CancellationToken.None;

        var req = ValidCreateRequest();
        req = new CreateBookingRequest
        {
            FlightScheduleId = req.FlightScheduleId,
            FareClass = "X", 
            BasePrice = req.BasePrice,
            Taxes = req.Taxes,
            Currency = req.Currency,
            IsRefundable = req.IsRefundable,
            Passengers = req.Passengers
        };

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Validation, result.ErrorType);

        _schedulesRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenScheduleNotFound_ReturnsNotFound()
    {
        var ct = CancellationToken.None;

        var req = ValidCreateRequest();

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FlightSchedule?)null);

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _ticketsRepo.Verify(r => r.CountByScheduleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCapacityUnknown_ReturnsValidation()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        var schedule = new FlightSchedule
        {
            Id = req.FlightScheduleId,
            FlightId = 10,
            AssignedAircraftId = null
        };

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _ticketsRepo
            .Setup(r => r.CountByScheduleAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _flightsRepo
            .Setup(r => r.GetByIdAsync(schedule.FlightId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Flight { Id = schedule.FlightId, DefaultAircraftId = null });

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Validation, result.ErrorType);

        _aircraftsRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenAircraftNotFound_ReturnsNotFound()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest();

        var schedule = new FlightSchedule
        {
            Id = req.FlightScheduleId,
            FlightId = 10,
            AssignedAircraftId = 99
        };

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _ticketsRepo
            .Setup(r => r.CountByScheduleAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _aircraftsRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aircraft?)null);

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenNotEnoughSeats_ReturnsConflict()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest(passengerCount: 2);

        var schedule = new FlightSchedule
        {
            Id = req.FlightScheduleId,
            FlightId = 10,
            AssignedAircraftId = 99
        };

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _ticketsRepo
            .Setup(r => r.CountByScheduleAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        _aircraftsRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aircraft { Id = 99, SeatCapacity = 5 });

        var result = await _sut.CreateAsync(req, ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_UsesAssignedAircraft_CreatesBookingAndTickets_ReturnsOk()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest(passengerCount: 2, currency: " eur "); 

        var schedule = new FlightSchedule
        {
            Id = req.FlightScheduleId,
            FlightId = 10,
            AssignedAircraftId = 99
        };

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _ticketsRepo
            .Setup(r => r.CountByScheduleAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _aircraftsRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aircraft { Id = 99, SeatCapacity = 100 });

        _bookingsRepo
            .Setup(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Booking? inserted = null;

        _bookingsRepo
            .Setup(r => r.InsertAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) =>
            {
                b.Id = 1;
                inserted = b;
            })
            .Returns(Task.CompletedTask);

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => inserted); 

        _ticketsRepo
            .Setup(r => r.CreateTicketsForBookingAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<FareClass>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IReadOnlyList<PassengerDto>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(req, ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        result.Value!.ConfirmationCode.ShouldNotBeNullOrWhiteSpace();
        Assert.Equal(6, result.Value.ConfirmationCode.Length);
        Assert.Equal("Active", result.Value.Status);
        Assert.Equal("EUR", result.Value.Currency);

        var expectedTotal = (req.BasePrice + req.Taxes) * req.Passengers.Count;
        Assert.Equal(expectedTotal, result.Value.TotalAmount);

        _uow.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        _bookingsRepo.Verify(r => r.InsertAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        _bookingsRepo.Verify(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _ticketsRepo.Verify(r => r.CreateTicketsForBookingAsync(
            1,
            req.FlightScheduleId,
            It.IsAny<FareClass>(),
            req.BasePrice,
            req.Taxes,
            "EUR",
            req.IsRefundable,
            It.Is<IReadOnlyList<PassengerDto>>(p => p.Count == req.Passengers.Count),
            It.IsAny<CancellationToken>()), Times.Once);

        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_WhenScheduleHasNoAssignedAircraft_UsesFlightDefaultAircraft_ReturnsOk()
    {
        var ct = CancellationToken.None;
        var req = ValidCreateRequest(passengerCount: 1);

        var schedule = new FlightSchedule
        {
            Id = req.FlightScheduleId,
            FlightId = 10,
            AssignedAircraftId = null
        };

        _schedulesRepo
            .Setup(r => r.GetByIdAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _ticketsRepo
            .Setup(r => r.CountByScheduleAsync(req.FlightScheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _flightsRepo
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Flight { Id = 10, DefaultAircraftId = 77 });

        _aircraftsRepo
            .Setup(r => r.GetByIdAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aircraft { Id = 77, SeatCapacity = 100 });

        _bookingsRepo
            .Setup(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Booking? inserted = null;

        _bookingsRepo
            .Setup(r => r.InsertAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) =>
            {
                b.Id = 1;
                inserted = b;
            })
            .Returns(Task.CompletedTask);

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => inserted);

        _ticketsRepo
            .Setup(r => r.CreateTicketsForBookingAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<FareClass>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IReadOnlyList<PassengerDto>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(req, ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(6, result.Value!.ConfirmationCode.Length);

        _flightsRepo.Verify(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        _aircraftsRepo.Verify(r => r.GetByIdAsync(77, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByCodeAsync_WhenMissing_ReturnsNull()
    {
        var ct = CancellationToken.None;

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync("X", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _sut.GetByCodeAsync("X", ct);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCodeAsync_WhenFound_ReturnsDto()
    {
        var ct = CancellationToken.None;

        var booking = new Booking
        {
            Id = 1,
            ConfirmationCode = "ABC123",
            Quantity = 2,
            Status = BookingStatus.Active,
            CreatedUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var result = await _sut.GetByCodeAsync("ABC123", ct);

        Assert.NotNull(result);
        Assert.Equal("ABC123", result!.ConfirmationCode);
        Assert.Equal("Active", result.Status);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(booking.CreatedUtc, result.CreatedUtc);
    }

    [Fact]
    public async Task CancelAsync_WhenBookingMissing_ReturnsNotFound()
    {
        var ct = CancellationToken.None;

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync("NOPE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var result = await _sut.CancelAsync("NOPE", ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        _bookingsRepo.Verify(r => r.CancelByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelAsync_WhenAlreadyCancelled_ReturnsConflict()
    {
        var ct = CancellationToken.None;

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking
            {
                Id = 1,
                ConfirmationCode = "ABC123",
                Status = BookingStatus.Cancelled,
                Quantity = 1
            });

        var result = await _sut.CancelAsync("ABC123", ct);

        Assert.False(result.Success);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        _bookingsRepo.Verify(r => r.CancelByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelAsync_WhenValid_CancelsAndSaves_ReturnsOk()
    {
        var ct = CancellationToken.None;

        _bookingsRepo
            .Setup(r => r.GetByCodeAsync("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking
            {
                Id = 1,
                ConfirmationCode = "ABC123",
                Status = BookingStatus.Active,
                Quantity = 1
            });

        _bookingsRepo
            .Setup(r => r.CancelByCodeAsync("ABC123", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CancelAsync("ABC123", ct);

        Assert.True(result.Success);

        _bookingsRepo.Verify(r => r.CancelByCodeAsync("ABC123", It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateBookingRequest ValidCreateRequest(int passengerCount = 1, string currency = "EUR")
    {
        var passengers = Enumerable.Range(1, passengerCount)
            .Select(i => new PassengerDto
            {
                FullName = $"Passenger {i}",
                Email = $"p{i}@test.com",
                PhoneNumber = "0700000000"
            })
            .ToList();

        return new CreateBookingRequest
        {
            FlightScheduleId = 123,
            FareClass = "Y",
            Currency = currency,
            BasePrice = 100m,
            Taxes = 20m,
            IsRefundable = false,
            Passengers = passengers
        };
    }
}

internal static class AssertExtensions
{
    public static void ShouldNotBeNullOrWhiteSpace(this string? s)
    {
        Assert.False(string.IsNullOrWhiteSpace(s));
    }
}