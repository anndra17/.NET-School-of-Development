namespace ReadingList.App.UI.Abstractions;

public interface IConsole
{
    void Write(string text);
    void WriteLine(string text);
    void WriteLine();

    Task<string?> ReadLineAsync(CancellationToken cancellationToken = default);

    Task<bool> PromptYesNoAsync(
            string question,
            bool defaultYes = false,
            CancellationToken cancellationToken = default);
}
