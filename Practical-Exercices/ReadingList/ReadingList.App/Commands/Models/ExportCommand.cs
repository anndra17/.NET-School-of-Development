using ReadingList.Application.Enums;
using ReadingList.CLI.Commands.Abstractions;

namespace ReadingList.CLI.Commands.Models;

public sealed record ExportCommand(ExportFormat Format, string Path) : ICommand;

