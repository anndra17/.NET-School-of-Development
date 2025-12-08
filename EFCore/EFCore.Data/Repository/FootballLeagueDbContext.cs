using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data.Repository;

public class FootballLeagueDbContext : DbContext
{
    private string _dbPath;

    public FootballLeagueDbContext()
    {
        var folder = Environment.SpecialFolder.ApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Combine(path, "FootballLeague_EfCore.db");
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coach { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Using SQL Server
        //optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;
        //      Initial Catalog=FotballLeague_EfCore; Encrypt=False");

        // Using SQLite
        optionsBuilder.UseSqlite("Data Source=FotballLeague_EfCore.db");
    }
}
