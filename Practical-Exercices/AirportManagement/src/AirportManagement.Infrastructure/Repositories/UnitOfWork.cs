using AirportManagement.Application.Abstractions.Repositories;
using AirportManagement.Application.Exceptions;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AirportManagementDbContext _context;

    public IAircraftRepository AircraftsRepository { get; } 
    public IAirlineRepository AirlinesRepository { get; }
    public IAirportRepository AirportsRepository { get; }
    public IBookingRepository BookingsRepository { get; }
    public IFlightRepository FlightsRepository { get; }
    public IFlightScheduleRepository FlightSchedulesRepository { get; }
    public IGateRepository GatesRepository { get; }
    public ITicketRepository TicketsRepository { get; }
    public IUserRepository UsersRepository { get; }

    public UnitOfWork(
       AirportManagementDbContext context,
       IAircraftRepository aircraftsRepository,
       IAirlineRepository airlinesRepository,
       IAirportRepository airportsRepository,
       IBookingRepository bookingsRepository,
       IFlightRepository flightsRepository,
       IFlightScheduleRepository flightSchedulesRepository,
       IGateRepository gatesRepository,
       ITicketRepository ticketsRepository,
       IUserRepository usersRepository)
    {
        _context = context;
        AircraftsRepository = aircraftsRepository;
        AirlinesRepository = airlinesRepository;
        AirportsRepository = airportsRepository;
        BookingsRepository = bookingsRepository;
        FlightsRepository = flightsRepository;
        FlightSchedulesRepository = flightSchedulesRepository;
        GatesRepository = gatesRepository;
        TicketsRepository = ticketsRepository;
        UsersRepository = usersRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictException("Database update conflict (constraint violation).", ex);
        }
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
