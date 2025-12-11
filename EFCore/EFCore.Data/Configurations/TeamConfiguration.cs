using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasData(
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
           }
        );
    }
}

