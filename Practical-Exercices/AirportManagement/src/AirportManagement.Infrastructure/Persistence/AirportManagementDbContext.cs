using System;
using System.Collections.Generic;
using AirportManagement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Persistence;

public partial class AirportManagementDbContext : DbContext
{
    public AirportManagementDbContext(DbContextOptions<AirportManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AircraftEntity> Aircrafts { get; set; }

    public virtual DbSet<AirlineEntity> Airlines { get; set; }

    public virtual DbSet<AirportEntity> Airports { get; set; }

    public virtual DbSet<BookingEntity> Bookings { get; set; }

    public virtual DbSet<FareOfferEntity> FareOffers { get; set; }

    public virtual DbSet<FlightEntity> Flights { get; set; }

    public virtual DbSet<FlightScheduleEntity> FlightSchedules { get; set; }

    public virtual DbSet<GateEntity> Gates { get; set; }

    public virtual DbSet<TicketEntity> Tickets { get; set; }

    public virtual DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AircraftEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Aircraft__3214EC0780DA1EB6");

            entity.HasIndex(e => e.TailNumber, "UQ__Aircraft__3F41D11B64CA276C").IsUnique();

            entity.Property(e => e.Model).HasMaxLength(60);
            entity.Property(e => e.TailNumber).HasMaxLength(10);
        });

        modelBuilder.Entity<AirlineEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airlines__3214EC0734929F01");

            entity.HasIndex(e => e.IATACode, "IX_Airlines_IATACode").IsUnique();

            entity.HasIndex(e => e.IATACode, "UQ__Airlines__EFD6F5BE600349D1").IsUnique();

            entity.Property(e => e.IATACode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<AirportEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airports__3214EC0708B8A672");

            entity.HasIndex(e => e.IATACode, "IX_Airports_IATACode").IsUnique();

            entity.HasIndex(e => e.IATACode, "UQ__Airports__EFD6F5BE152E8713").IsUnique();

            entity.Property(e => e.City).HasMaxLength(80);
            entity.Property(e => e.Country).HasMaxLength(80);
            entity.Property(e => e.IATACode)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.TimeZone).HasMaxLength(64);
        });

        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC072660FC0B");

            entity.HasIndex(e => e.ConfirmationCode, "IX_Bookings_ConfirmationCode").IsUnique();

            entity.HasIndex(e => e.ConfirmationCode, "UQ__Bookings__19683086E2E91C3B").IsUnique();

            entity.Property(e => e.ConfirmationCode).HasMaxLength(8);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<FareOfferEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FareOffe__3214EC075AC9512E");

            entity.HasIndex(e => e.FlightScheduleId, "IX_FareOffers_FlightScheduleId");

            entity.HasIndex(e => new { e.FlightScheduleId, e.FareClass }, "UQ_FareOffer_Schedule_FareClass").IsUnique();

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.FareClass).HasMaxLength(2);
            entity.Property(e => e.Taxes).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([BasePrice]+[Taxes])", true)
                .HasColumnType("decimal(11, 2)");

            entity.HasOne(d => d.FlightSchedule).WithMany(p => p.FareOffers)
                .HasForeignKey(d => d.FlightScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FareOffer_FlightSchedule");
        });

        modelBuilder.Entity<FlightEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Flights__3214EC076EA29E2B");

            entity.HasIndex(e => new { e.AirlineId, e.FlightNumber }, "IX_Flights_AirlineId_FlightNumber");

            entity.HasIndex(e => e.Id, "IX_Flights_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => new { e.OriginAirportId, e.DestinationAirportId }, "IX_Flights_OriginAirportId_DestinationAirportId");

            entity.Property(e => e.FlightNumber).HasMaxLength(8);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Airline).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_Airline");

            entity.HasOne(d => d.DefaultAircraft).WithMany(p => p.Flights)
                .HasForeignKey(d => d.DefaultAircraftId)
                .HasConstraintName("FK_Flight_DefaultAircraft");

            entity.HasOne(d => d.DestinationAirport).WithMany(p => p.FlightDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_DestinationAirport");

            entity.HasOne(d => d.OriginAirport).WithMany(p => p.FlightOriginAirports)
                .HasForeignKey(d => d.OriginAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_OriginAirport");
        });

        modelBuilder.Entity<FlightScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlightSc__3214EC075A2ACF2B");

            entity.HasIndex(e => new { e.FlightId, e.ScheduledDepartureUtc }, "IX_FlightSchedules_FlightId_ScheduledDepartureUtc");

            entity.HasIndex(e => new { e.GateId, e.ScheduledDepartureUtc }, "IX_FlightSchedules_Gate_ScheduledDeparture")
                .IsUnique()
                .HasFilter("([GateId] IS NOT NULL)");

            entity.HasOne(d => d.AssignedAircraft).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.AssignedAircraftId)
                .HasConstraintName("FK_FlightSchedule_AssignedAircraft");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlightSchedule_Flight");

            entity.HasOne(d => d.Gate).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.GateId)
                .HasConstraintName("FK_FlightSchedule_Gate");
        });

        modelBuilder.Entity<GateEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gates__3214EC071502E543");

            entity.HasIndex(e => new { e.AirportId, e.Code }, "IX_Gates_AirportId_Code").IsUnique();

            entity.HasIndex(e => new { e.AirportId, e.Code }, "UQ_Gate_Airport_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(10);

            entity.HasOne(d => d.Airport).WithMany(p => p.Gates)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gate_Airport");
        });

        modelBuilder.Entity<TicketEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3214EC074FA6D07A");

            entity.HasIndex(e => e.BookingId, "IX_Tickets_BookingId");

            entity.HasIndex(e => e.FareOfferId, "IX_Tickets_FareOfferId");

            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.PassengerEmail).HasMaxLength(60);
            entity.Property(e => e.PassengerFullName).HasMaxLength(120);
            entity.Property(e => e.PassengerPhoneNumber).HasMaxLength(60);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Booking");

            entity.HasOne(d => d.FareOffer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FareOfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_FareOffer");
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07667FEE7D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CABF818E").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(60);
            entity.Property(e => e.FullName).HasMaxLength(120);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
