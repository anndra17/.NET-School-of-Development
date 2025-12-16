using EFCore.Data.Configurations;
using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EFCore.Data.Repository;

public class FootballLeagueDbContext : DbContext
{
    private readonly string _dbPath;
    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<League> Leagues { get; set; }

    public FootballLeagueDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Combine(path, "FootballLeague_EfCore.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Using SQL Server
        //optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;
        //      Initial Catalog=FotballLeague_EfCore; Encrypt=False");

        // Using SQLite
        optionsBuilder.UseSqlite($"Data Source={_dbPath}")
            .UseLazyLoadingProxies()
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfiguration(new TeamConfiguration());
        //modelBuilder.ApplyConfiguration(new LeagueConfiguration());

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
