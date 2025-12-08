using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data.Repository;

public class FootballLeagueDbContext : DbContext
{
    private readonly string _dbPath;

    public FootballLeagueDbContext()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _dbPath = Path.Combine(folder, "FootballLeague_EfCore.db");
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coaches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Using SQL Server
        //optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;
        //      Initial Catalog=FotballLeague_EfCore; Encrypt=False");

        // Using SQLite
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}
