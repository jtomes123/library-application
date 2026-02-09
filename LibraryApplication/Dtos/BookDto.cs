namespace LibraryApplication.Dtos;

public record BookDto(Guid Id, string Name, string Author, int Year, string Isbn, int AvailableCopies)
{
    public static BookDto Empty => new BookDto(Guid.Empty, string.Empty, string.Empty, 2000, string.Empty, 0);
};