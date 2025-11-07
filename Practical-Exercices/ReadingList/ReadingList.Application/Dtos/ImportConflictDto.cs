using ReadingList.Domain.Entities;

namespace ReadingList.Application.Dtos;

public sealed record ImportConflictDto(
    int Id, 
    Book Existing,
    Book Incoming, 
    string SourcePath, 
    int LineNumber);



