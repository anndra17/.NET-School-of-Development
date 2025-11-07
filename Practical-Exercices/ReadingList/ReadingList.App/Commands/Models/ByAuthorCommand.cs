using ReadingList.CLI.Commands.Abstractions;

namespace ReadingList.CLI.Commands.Models;

public sealed record ByAuthorCommand(string Text, bool IgnoreCase) : ICommand;

