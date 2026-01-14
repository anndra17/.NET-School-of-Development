using System.Text.Json.Serialization;

namespace AirportManagement.Application.Dtos.Schedule;

public sealed class ImportSchedulesResponseDto
{
    public int Total { get; init; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public List<ImportRowErrorDto> Errors { get; init; } = new();
}