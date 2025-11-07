namespace ReadingList.Application.Abstractions.Logs;

public interface ISystemLogger
{
    void Info(string message);

    void Warn(string message);

    void Error(string message);
}
