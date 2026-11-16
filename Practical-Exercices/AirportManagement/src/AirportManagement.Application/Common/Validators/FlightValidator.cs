namespace AirportManagement.Application.Common.Validators;

public static class FlightValidator
{
    public static bool IsValidFlightNumber(this string flightNumber)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            return false;

        if (flightNumber.Length != 6)
            return false;

        return char.IsUpper(flightNumber[0]) &&
               char.IsUpper(flightNumber[1]) &&
               char.IsDigit(flightNumber[2]) &&
               char.IsDigit(flightNumber[3]) &&
               char.IsDigit(flightNumber[4]) &&
               char.IsDigit(flightNumber[5]);
    }
}
