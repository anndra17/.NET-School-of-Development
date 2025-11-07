namespace ReadingList.CLI.UI.Abstractions;

public interface IConsoleNotifier
{
    void Info(string message);

    void Success(string message);

    void Warn(string message);

    void Error(string message);
}
