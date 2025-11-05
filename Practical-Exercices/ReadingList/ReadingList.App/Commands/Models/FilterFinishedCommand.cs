using ReadingList.App.Commands.Abstractions;

namespace ReadingList.App.Commands.Models;

public sealed record FilterFinishedCommand(bool Finished) : ICommand;
