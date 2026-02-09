using System.ComponentModel.DataAnnotations;

namespace LibraryApplication.Models;

public class User: BaseModel
{
    public Guid Id { get; set; }
    [MaxLength(128)]
    public required string Name { get; set; }
    [MaxLength(128)]
    public required string Email { get; set; }

    public List<HistoryEntry> History { get; set; } = new();
}