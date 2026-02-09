using LibraryApplication.Interfaces;
using LibraryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Services;

public class LibraryDbContext: DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<HistoryEntry> HistoryEntries { get; set; }
    public DbSet<User> Users { get; set; }
    
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }
    
    public override int SaveChanges()
    {
        UpdateConcurrencyTokens();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateConcurrencyTokens();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateConcurrencyTokens()
    {
        var entries = ChangeTracker.Entries<IModelConcurrency>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Property(nameof(IModelConcurrency.Version)).CurrentValue = Guid.NewGuid();
        }
    }
}