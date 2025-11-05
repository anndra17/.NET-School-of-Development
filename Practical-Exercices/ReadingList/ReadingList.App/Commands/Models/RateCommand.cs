using ReadingList.App.Commands.Abstractions;

namespace ReadingList.App.Commands.Models;

public sealed record RateCommand(int Id, double Rating) : ICommand;
