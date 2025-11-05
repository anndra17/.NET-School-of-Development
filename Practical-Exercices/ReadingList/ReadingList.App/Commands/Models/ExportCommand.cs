using ReadingList.App.Commands.Abstractions;
using ReadingList.Infrastructure.Enums;

namespace ReadingList.App.Commands.Models;

public sealed record ExportCommand(ExportFormat Format, string Path) : ICommand;

