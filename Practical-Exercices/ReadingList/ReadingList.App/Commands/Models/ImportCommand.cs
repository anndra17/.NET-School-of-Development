using ReadingList.App.Commands.Abstractions;

namespace ReadingList.App.Commands.Models;

public sealed record ImportCommand(IReadOnlyList<string> Paths) : ICommand;
