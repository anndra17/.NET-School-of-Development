namespace AirportManagement.Application.Dtos.Schedule;


public sealed class ImportRowErrorDto
{
    public int Row { get; init; } 
    public string Message { get; init; } = null!;
}