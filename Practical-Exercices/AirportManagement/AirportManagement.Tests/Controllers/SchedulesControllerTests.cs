using AirportManagement.API.Controllers;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Schedule;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Text;

namespace AirportManagement.Tests.Controllers;

public class SchedulesControllerTests
{
    private readonly Mock<IFlightScheduleService> _service = new();
    private readonly SchedulesController _controller;

    public SchedulesControllerTests()
    {
        _controller = new SchedulesController(_service.Object);
    }

    [Fact]
    public async Task Import_WhenSomeRowsFail_Returns207MultiStatus()
    {
        var ct = CancellationToken.None;

        var importResponse = new ImportSchedulesResponseDto
        {
            Total = 2,
            Created = 1,
            Updated = 0,
        };
        importResponse.Errors.Add(new ImportRowErrorDto { Row = 2, Message = "Gate overlap" });

        _service
            .Setup(s => s.ImportAsync(It.IsAny<Stream>(), ct))
            .ReturnsAsync(Result<ImportSchedulesResponseDto>.Ok(importResponse));

        var file = CreateJsonFormFile("schedules.json", "[{},{ }]");

        var actionResult = await _controller.Import(file, ct);

        var objectResult = actionResult.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status207MultiStatus);
        objectResult.Value.Should().BeSameAs(importResponse);

        _service.Verify(s => s.ImportAsync(It.IsAny<Stream>(), ct), Times.Once);
    }

    [Fact]
    public async Task Import_WhenAllRowsPass_Returns201Created()
    {
        var ct = CancellationToken.None;

        var importResponse = new ImportSchedulesResponseDto
        {
            Total = 2,
            Created = 2,
            Updated = 0,
        };

        _service
            .Setup(s => s.ImportAsync(It.IsAny<Stream>(), ct))
            .ReturnsAsync(Result<ImportSchedulesResponseDto>.Ok(importResponse));

        var file = CreateJsonFormFile("schedules.json", "[{},{}]");

        var actionResult = await _controller.Import(file, ct);

        var objectResult = actionResult.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status201Created);
        objectResult.Value.Should().BeSameAs(importResponse);

        _service.Verify(s => s.ImportAsync(It.IsAny<Stream>(), ct), Times.Once);
    }

    private static IFormFile CreateJsonFormFile(string fileName, string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/json"
        };
    }
}
