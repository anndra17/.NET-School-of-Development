namespace AirportManagement.Application.Abstractions;

public interface IUnitOfWork
{
    IAircraftRepository Aircrafts { get; }
    IAirlineRepository Airlines { get; }
    IAirportRepository Airports { get; }
    IBookingRepository Bookings { get; }
    IFlightRepository Flights { get; }
    IFlightScheduleRepository FlightSchedules { get; }
    IGateRepository Gates { get; }
    ITicketRepository Tickets { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
