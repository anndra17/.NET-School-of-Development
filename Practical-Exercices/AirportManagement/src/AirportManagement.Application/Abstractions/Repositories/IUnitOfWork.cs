namespace AirportManagement.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    IAircraftRepository AircraftsRepository { get; }
    IAirlineRepository AirlinesRepository { get; }
    IAirportRepository AirportsRepository { get; }
    IBookingRepository BookingsRepository { get; }
    IFlightRepository FlightsRepository { get; }
    IFlightScheduleRepository FlightSchedulesRepository { get; }
    IGateRepository GatesRepository { get; }
    IFareOfferRepository FareOffersRepository { get; }
    ITicketRepository TicketsRepository { get; }
    IUserRepository UsersRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
