namespace LibraryApplication.Models;

public class HistoryEntry: BaseModel
{
    public Guid Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public HistoryAction Action { get; set; }
    public Guid BookCopyId { get; set; }
    public Guid UserId { get; set; }

    public BookCopy BookCopy { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum HistoryAction
{
    Registered,
    Borrowed,
    Returned
}