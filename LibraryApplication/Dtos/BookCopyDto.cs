namespace LibraryApplication.Dtos;

public record BookCopyDto(Guid Id, string Name, string Author, int Year, string Isbn, string? Borrower, bool IsAvailable);