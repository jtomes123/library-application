using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Models;

[Index(nameof(Name), nameof(Author), nameof(Isbn))]
public class Book: BaseModel
{
    public Guid Id { get; set; }
    [MaxLength(128)]
    public required string Name { get; set; }
    [MaxLength(128)]
    public required string Author { get; set; }
    [MaxLength(13)]
    public required string Isbn { get; set; }
    public int Year { get; set; }
    public bool IsDeleted { get; set; }

    public List<BookCopy> Copies { get; set; } = new();
}