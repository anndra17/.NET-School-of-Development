using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations;

internal class CoachConfiguration : IEntityTypeConfiguration<Coach>
{
    public void Configure(EntityTypeBuilder<Coach> builder)
    {
        builder.HasData(
            new Coach
            {
                Id = 1,
                Name = "Alesandro Santos"
            },
            new Coach
            {
                Id = 2,
                Name = "Ion Saftoiu"
            },
            new Coach
            {
                Id = 3,
                Name = "Vasile Bragadiru"
            }
            );
    }
}
