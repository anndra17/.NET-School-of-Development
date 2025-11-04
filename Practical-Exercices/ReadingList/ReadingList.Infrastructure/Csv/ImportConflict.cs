using ReadingList.Domain.Entities;

namespace ReadingList.Infrastructure.Csv;

public sealed record ImportConflict(
    int Id, 
    Book Existing,
    Book Incoming, 
    string SourcePath, 
    int LineNumber);



