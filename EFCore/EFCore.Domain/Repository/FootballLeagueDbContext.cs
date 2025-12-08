using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data.Repository;

public class FootballLeagueDbContext : DbContext
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
