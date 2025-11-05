using ReadingList.App.Commands.Abstractions;

namespace ReadingList.App.Commands.Models;

public sealed record MarkFinishedCommand(int Id) : ICommand;
