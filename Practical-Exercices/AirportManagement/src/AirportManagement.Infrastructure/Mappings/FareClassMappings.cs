using AirportManagement.Domain.Enums;

namespace AirportManagement.Infrastructure.Mappings;

internal static class FareClassMappings
{
    public static FareClass ToDomain(string dbValue)
        => dbValue switch
        {
            "Y" => FareClass.Economy,
            "M" => FareClass.PremiumEconomy,
            "J" => FareClass.Business,
            "F" => FareClass.First,
            _ => throw new InvalidOperationException($"Unknown FareClass value '{dbValue}'")
        };

    public static string ToEntity(FareClass domainValue)
        => domainValue switch
        {
            FareClass.Economy => "Y",
            FareClass.PremiumEconomy => "M",
            FareClass.Business => "J",
            FareClass.First => "F",
            _ => throw new InvalidOperationException($"Unknown FareClass enum '{domainValue}'")
        };
}