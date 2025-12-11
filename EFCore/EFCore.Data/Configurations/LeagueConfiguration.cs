using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations;

internal class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.HasData(
                   new League
                   {
                       Id = 4,
                       Name = "Jamaica Premiere League",
                   },
                   new League
                   {
                       Id = 5,
                       Name = "English Premiere League",
                   },
                   new League
                   {
                       Id = 6,
                       Name = "Romania Premiere League",
                   }
        );
    }
}
