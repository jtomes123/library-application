namespace LibraryApplication.Models;

public class BookCopy: BaseModel
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public Book Book { get; set; } = null!;
    public List<HistoryEntry> History { get; set; } = new();
}