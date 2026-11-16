using AirportManagement.API.Controllers;
using AirportManagement.Application.Abstractions.Services;
using AirportManagement.Application.Common.Paging;
using AirportManagement.Application.Common.Results;
using AirportManagement.Application.Dtos.Schedule;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

    [Fact]
    public async Task Search_WhenFiltersProvided_Returns200OkWithPagedResult_AndCallsServiceWithSameQuery()
    {
        var ct = CancellationToken.None;

        var query = new ScheduleSearchQuery
        {
            Origin = "OTP",
            Destination = "LHR",
            Date = new DateOnly(2026, 01, 12),
            Page = 2,
            PageSize = 10
        };

        var paged = new PagedResponse<ScheduleListItemResponse>
        {
            Page = 2,
            PageSize = 10,
            TotalItems = 25,
            TotalPages = 3,
            Items = new List<ScheduleListItemResponse>
        {
            new ScheduleListItemResponse(), 
            new ScheduleListItemResponse()
        }
        };

        _service
            .Setup(s => s.SearchAsync(
                It.Is<ScheduleSearchQuery>(q =>
                    q.Origin == "OTP" &&
                    q.Destination == "LHR" &&
                    q.Date == new DateOnly(2026, 01, 12) &&
                    q.Page == 2 &&
                    q.PageSize == 10
                ),
                ct))
            .ReturnsAsync(paged);

        var actionResult = await _controller.Search(query, ct);

        var ok = actionResult.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().BeSameAs(paged);

        _service.Verify(s => s.SearchAsync(It.IsAny<ScheduleSearchQuery>(), ct), Times.Once);
    }
}
