namespace AirportManagement.Domain;

public enum FlightScheduleStatus : byte
{
    Planned, 
    Boarding,
    Departed,
    Cancelled,
    Delayed
}
