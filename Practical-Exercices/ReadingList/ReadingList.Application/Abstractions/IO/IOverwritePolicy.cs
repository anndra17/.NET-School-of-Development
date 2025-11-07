namespace ReadingList.Application.Abstractions.IO;

public interface IOverwritePolicy
{
    Task<bool> ConfirmOverwriteAsync(string path, CancellationToken cancellationToken);
}
