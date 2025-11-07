using ReadingList.Application.Abstractions.IO;
using ReadingList.CLI.UI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.CLI.Adapters;

public sealed class ConsoleOverwritePolicy : IOverwritePolicy
{
    private readonly IConsole _console;
    public ConsoleOverwritePolicy(IConsole console) => _console = console;

    public Task<bool> ConfirmOverwriteAsync(string path, CancellationToken cancellationToken)
        => _console.PromptYesNoAsync($"File \"{path}\" already exists. Overwrite?", defaultYes: false, cancellationToken);
}