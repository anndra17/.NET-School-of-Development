namespace ReadingList.Infrastructure.Interfaces;

public interface IOverwritePolicy
{
    Task<bool> ConfirmOverwriteAsync(string path, CancellationToken cancellationToken);
}
