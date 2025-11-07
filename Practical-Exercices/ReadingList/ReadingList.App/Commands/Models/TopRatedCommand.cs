using ReadingList.CLI.Commands.Abstractions;

namespace ReadingList.CLI.Commands.Models;

public sealed record TopRatedCommand(int Count) : ICommand;

