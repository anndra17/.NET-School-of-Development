using AirportManagement.Application.Abstractions;
using AirportManagement.Infrastructure.Persistence;

namespace AirportManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AirportManagementDbContext _context;

    public IAircraftRepository Aircrafts { get; }
    public IAirlineRepository Airlines { get; }
    public IAirportRepository Airports { get; }
    public IBookingRepository Bookings { get; }
    public IFlightRepository Flights { get; }
    public IFlightScheduleRepository FlightSchedules { get; }
    public IGateRepository Gates { get; }
    public ITicketRepository Tickets { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(
       AirportManagementDbContext context,
       IAircraftRepository aircrafts,
       IAirlineRepository airlines,
       IAirportRepository airports,
       IBookingRepository bookings,
       IFlightRepository flights,
       IFlightScheduleRepository flightSchedules,
       IGateRepository gates,
       ITicketRepository tickets,
       IUserRepository users)
    {
        _context = context;
        Aircrafts = aircrafts;
        Airlines = airlines;
        Airports = airports;
        Bookings = bookings;
        Flights = flights;
        FlightSchedules = flightSchedules;
        Gates = gates;
        Tickets = tickets;
        Users = users;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await action(ct);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
