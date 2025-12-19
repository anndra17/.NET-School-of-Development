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

    public virtual DbSet<Aircraft> Aircrafts { get; set; }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightSchedule> FlightSchedules { get; set; }

    public virtual DbSet<Gate> Gates { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Aircraft__3214EC072737F6AB");

            entity.HasIndex(e => e.TailNumber, "UQ__Aircraft__3F41D11B860337C9").IsUnique();

            entity.Property(e => e.Model).HasMaxLength(60);
            entity.Property(e => e.TailNumber).HasMaxLength(10);
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airlines__3214EC0741BF4E31");

            entity.HasIndex(e => e.IATACode, "IX_Airlines_IATACode").IsUnique();

            entity.HasIndex(e => e.IATACode, "UQ__Airlines__EFD6F5BE3CF31D6F").IsUnique();

            entity.Property(e => e.IATACode)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airports__3214EC070E3A52D7");

            entity.HasIndex(e => e.IATACode, "IX_Airports_IATACode").IsUnique();

            entity.HasIndex(e => e.IATACode, "UQ__Airports__EFD6F5BEAD98D279").IsUnique();

            entity.Property(e => e.City).HasMaxLength(80);
            entity.Property(e => e.Country).HasMaxLength(80);
            entity.Property(e => e.IATACode)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.TimeZone).HasMaxLength(64);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07A513937C");

            entity.HasIndex(e => e.ConfirmationCode, "IX_Bookings_ConfirmationCode").IsUnique();

            entity.HasIndex(e => e.ConfirmationCode, "UQ__Bookings__196830869DB9AB28").IsUnique();

            entity.Property(e => e.ConfirmationCode).HasMaxLength(8);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Flights__3214EC07EF17F4F0");

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

        modelBuilder.Entity<FlightSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlightSc__3214EC07AE643BAB");

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

        modelBuilder.Entity<Gate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gates__3214EC07E8BE8E8C");

            entity.HasIndex(e => new { e.AirportId, e.Code }, "IX_Gate_AirportId_Code").IsUnique();

            entity.HasIndex(e => new { e.AirportId, e.Code }, "UQ_Gate_Airport_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(10);

            entity.HasOne(d => d.Airport).WithMany(p => p.Gates)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gate_Airport");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3214EC07EB9B57D4");

            entity.HasIndex(e => new { e.FlightScheduleId, e.FareClass }, "IX_Tickets_FlightScheduleId_FareClass");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.FareClass).HasMaxLength(2);
            entity.Property(e => e.PassengerEmail).HasMaxLength(60);
            entity.Property(e => e.PassengerFullName).HasMaxLength(120);
            entity.Property(e => e.PassengerPhoneNumber).HasMaxLength(60);
            entity.Property(e => e.Taxes).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([BasePrice]+[Taxes])", true)
                .HasColumnType("decimal(11, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Booking");

            entity.HasOne(d => d.FlightSchedule).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_FlightSchedule");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07E5F9FC72");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F46E5D3D").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(60);
            entity.Property(e => e.FullName).HasMaxLength(120);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
