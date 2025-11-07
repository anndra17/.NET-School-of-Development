using ReadingList.Application.Abstractions.Persistence;
using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;
using ReadingList.Domain.Entities;
using System.Globalization;

namespace ReadingList.CLI.Commands.Routing;

public sealed class UpdateHandler : IUpdateHandler
{
    private readonly IRepository<Book, int> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConsoleNotifier _consoleNotifier;

    public UpdateHandler(
        IRepository<Book, int> repository,
        IUnitOfWork unitOfWork,
        IConsoleNotifier consoleNotifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public async Task HandleAsync(MarkFinishedCommand command, CancellationToken ct)
    {
        if (!_repository.TryGet(command.Id, out var book) || book is null)
        {
            _consoleNotifier.Error($"404 NOT FOUND: book id {command.Id.ToString(CultureInfo.InvariantCulture)}.");
            return;
        }

        if (!book.Finished) 
        {
            book.MarkFinished();
            _repository.Upsert(book);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            if (!saved.IsSuccess)
            {
                _consoleNotifier.Error($"Failed to save changes: {saved.Error}");
                return;
            }

        }

        _consoleNotifier.Success($"200 OK: book {book.Id} marked as finished.");
    }

    public async Task HandleAsync(RateCommand command, CancellationToken ct)
    {
        if (command.Rating < 0 || command.Rating > 5)
        {
            _consoleNotifier.Error("400 BAD REQUEST: rating must be between 0 and 5.");
            return;
        }

        if (!_repository.TryGet(command.Id, out var book) || book is null)
        {
            _consoleNotifier.Error($"404 NOT FOUND: book id {command.Id.ToString(CultureInfo.InvariantCulture)}.");
            return;
        }

        book.SetRating(command.Rating);

        _repository.Upsert(book);
        var saved = await _unitOfWork.SaveChangesAsync(ct);
        if (!saved.IsSuccess)
        {
            _consoleNotifier.Error($"Failed to save changes: {saved.Error}");
            return;
        }

        _consoleNotifier.Success($"200 OK: book {book.Id} rated {command.Rating.ToString("0.##", CultureInfo.InvariantCulture)}.");
    }
}
