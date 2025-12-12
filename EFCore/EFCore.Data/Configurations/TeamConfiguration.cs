using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasIndex(q => q.Name).IsUnique();

        builder.HasMany(m => m.HomeMatches) // tu ai mm homematches
            .WithOne(q => q.HomeTeam)       // bazat pe prop de navigare HomeTeam
            .HasForeignKey(q => q.HomeTeamId)  // FK - info share uita
            .IsRequired()                      // n ar rebui sa fie nullable
            .OnDelete(DeleteBehavior.Restrict);// o echipa nu poate fi stearsa daca are vreun HomeMatches

        builder.HasMany(m => m.AwayMatches)
            .WithOne(n => n.AwayTeam)
            .HasForeignKey(q => q.AwayTeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasData(
           new Team
           {
               Id = 1,
               Name = "Tivoli Gardens FC",
               CreatedDate = new DateTime(2024, 01, 01),
               LeagueId = 1,
               CoachId = 1,
           },
           new Team
           {
               Id = 2,
               Name = "Dinamo FC",
               CreatedDate = new DateTime(2025, 04, 01),
               LeagueId = 1,
               CoachId = 2,
           },
           new Team
           {
               Id = 3,
               Name = "FCSDB",
               CreatedDate = new DateTime(2004, 01, 01),
               LeagueId = 1,
               CoachId = 3,
           }
        );
    }
}

