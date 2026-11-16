using AirportManagement.Infrastructure.Persistence;
using AirportManagement.Infrastructure.Persistence.Entities;
using AirportManagement.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace AirportManagement.Tests.Repositories;

public class SchedulesRepositoryTests
{
    private static async Task<(AirportManagementDbContext db, SqliteConnection conn)> CreateDbAsync()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        await conn.OpenAsync();

        var options = new DbContextOptionsBuilder<AirportManagementDbContext>()
            .UseSqlite(conn)
            .Options;

        var db = new AirportManagementDbContext(options);

        await db.Database.EnsureCreatedAsync();

        return (db, conn);
    }

    private static async Task SeedAsync(AirportManagementDbContext db)
    {
        var otp = new AirportEntity { Id = 1, IATACode = "OTP", Name = "Otopeni", City = "Bucharest", Country = "RO", TimeZone = "Europe/Bucharest" };
        var lhr = new AirportEntity { Id = 2, IATACode = "LHR", Name = "Heathrow", City = "London", Country = "UK", TimeZone = "Europe/London" };
        var cdg = new AirportEntity { Id = 3, IATACode = "CDG", Name = "Charles de Gaulle", City = "Paris", Country = "FR", TimeZone = "Europe/Paris" };

        db.Airports.AddRange(otp, lhr, cdg);

        var airline = new AirlineEntity { Id = 1, IATACode = "BA", Name = "British Airways" };
        db.Airlines.Add(airline);

        var f1 = new FlightEntity
        {
            Id = 10,
            AirlineId = airline.Id,
            Airline = airline,
            FlightNumber = "BA100",
            OriginAirportId = otp.Id,
            OriginAirport = otp,
            DestinationAirportId = lhr.Id,
            DestinationAirport = lhr,
            IsActive = true
        };

        var f2 = new FlightEntity
        {
            Id = 11,
            AirlineId = airline.Id,
            Airline = airline,
            FlightNumber = "BA200",
            OriginAirportId = otp.Id,
            OriginAirport = otp,
            DestinationAirportId = cdg.Id,
            DestinationAirport = cdg,
            IsActive = true
        };

        var f3 = new FlightEntity
        {
            Id = 12,
            AirlineId = airline.Id,
            Airline = airline,
            FlightNumber = "BA300",
            OriginAirportId = cdg.Id,
            OriginAirport = cdg,
            DestinationAirportId = lhr.Id,
            DestinationAirport = lhr,
            IsActive = true
        };

        db.Flights.AddRange(f1, f2, f3);

        db.FlightSchedules.AddRange(
            new FlightScheduleEntity
            {
                Id = 100,
                FlightId = f1.Id,
                Flight = f1,
                ScheduledDepartureUtc = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc = new DateTime(2026, 1, 12, 12, 0, 0, DateTimeKind.Utc),
                Status = 0
            },
            new FlightScheduleEntity
            {
                Id = 101,
                FlightId = f1.Id,
                Flight = f1,
                ScheduledDepartureUtc = new DateTime(2026, 1, 12, 12, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc = new DateTime(2026, 1, 12, 14, 0, 0, DateTimeKind.Utc),
                Status = 0
            },
            new FlightScheduleEntity
            {
                Id = 102,
                FlightId = f2.Id,
                Flight = f2,
                ScheduledDepartureUtc = new DateTime(2026, 1, 12, 9, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
                Status = 0
            },
            new FlightScheduleEntity
            {
                Id = 103,
                FlightId = f3.Id,
                Flight = f3,
                ScheduledDepartureUtc = new DateTime(2026, 1, 13, 9, 0, 0, DateTimeKind.Utc),
                ScheduledArrivalUtc = new DateTime(2026, 1, 13, 11, 0, 0, DateTimeKind.Utc),
                Status = 0
            }
        );

        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task SearchAsync_WhenOriginProvided_FiltersByOriginIata()
    {
        var (db, conn) = await CreateDbAsync();
        await using (db)
        await using (conn)
        {
            await SeedAsync(db);

            var repo = new FlightScheduleRepository(db); 

            var (items, total) = await repo.SearchAsync(
                originIata: " otp ",      
                destinationIata: null,
                date: null,
                page: 1,
                pageSize: 20,
                ct: CancellationToken.None);

            total.Should().Be(3);
            items.Should().HaveCount(3);

            items.Should().OnlyContain(i => i.OriginIataCode == "OTP");
        }
    }
}
