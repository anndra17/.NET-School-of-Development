using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.Data.Repository;

public class FootballLeagueDbContext : DbContext
{
    private readonly string _dbPath;
    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coaches { get; set; }

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
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>().HasData(
            new Team
            {
                Id = 1,
                Name = "Tivoli Gardens FC",
                CreatedDate = new DateTime(2024, 01, 01),
            },
            new Team
            {
                Id = 2,
                Name = "Dinamo FC",
                CreatedDate = new DateTime(2025, 04, 01),
            },
            new Team
            {
                Id = 3,
                Name = "FCSDB",
                CreatedDate = new DateTime(2004, 01, 01),
            });
    }
}
