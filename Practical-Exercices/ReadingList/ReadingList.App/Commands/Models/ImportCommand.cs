using ReadingList.CLI.Commands.Abstractions;

namespace ReadingList.CLI.Commands.Models;

public sealed record ImportCommand(IReadOnlyList<string> Paths) : ICommand;
