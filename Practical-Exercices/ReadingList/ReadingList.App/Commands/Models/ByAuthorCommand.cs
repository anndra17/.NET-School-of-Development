using ReadingList.App.Commands.Abstractions;

namespace ReadingList.App.Commands.Models;

public sealed record ByAuthorCommand(string Text, bool IgnoreCase) : ICommand;

