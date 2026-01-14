using AirportManagement.Domain.Enums;

namespace AirportManagement.Application.Common.Converters;

internal static class FareClassConverter
{
    public static bool TryParse(string? input, out FareClass fareClass)
    {
        fareClass = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        return input.Trim().ToUpperInvariant() switch
        {
            "Y" => Return(FareClass.Economy, out fareClass),
            "M" => Return(FareClass.PremiumEconomy, out fareClass),
            "J" => Return(FareClass.Business, out fareClass),
            "F" => Return(FareClass.First, out fareClass),
            _ => false
        };

        static bool Return(FareClass value, out FareClass target)
        {
            target = value;
            return true;
        }
    }
}