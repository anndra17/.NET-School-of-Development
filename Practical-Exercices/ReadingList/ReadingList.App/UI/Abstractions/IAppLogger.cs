namespace ReadingList.App.UI.Abstractions;

public interface IAppLogger
{
    void Info(string message);

    void Success(string message);

    void Warn(string message);

    void Error(string message);
}
