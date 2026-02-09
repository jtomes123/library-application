using LibraryApplication.Dtos;
using LibraryApplication.Models;
using LibraryApplication.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Extensions;

public static class LibraryDbContextExtension
{
    extension(LibraryDbContext context)
    {
        public IQueryable<BookDto> GetBooksQueryable(string? searchTerm = null)
        {
            var query = context.Books.Where(b => !b.IsDeleted);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.Name.ToLower().Contains(lowerSearchTerm) ||
                    b.Author.ToLower().Contains(lowerSearchTerm) ||
                    b.Isbn.ToLower().Contains(lowerSearchTerm));
            }

            return query.Select(b => new BookDto(
                b.Id,
                b.Name,
                b.Author,
                b.Year,
                b.Isbn,
                b.Copies.Count(c => c.IsAvailable)
            ));
        }


        public IQueryable<BookCopyDto> GetBookCopiesByBookQueryable(Guid bookId) =>
            context.BookCopies
                .Where(bc => bc.BookId == bookId && !bc.IsDeleted)
                .Select(bc => new BookCopyDto(
                    bc.Id,
                    bc.Book.Name,
                    bc.Book.Author,
                    bc.Book.Year,
                    bc.Book.Isbn,
                    bc.IsAvailable
                        ? null
                        : bc.History
                            .OrderByDescending(he => he.TimeStamp)
                            .Where(he => he.Action == HistoryAction.Borrowed)
                            .Select(he => he.User.Name)
                            .FirstOrDefault(),
                    bc.IsAvailable
                ));

        public IQueryable<BookCopyDto> GetBookCopiesByUserQueryable(Guid userId) =>
            context.BookCopies
                .Where(bc => !bc.IsAvailable && !bc.IsDeleted &&
                             bc.History
                                 .OrderByDescending(he => he.TimeStamp)
                                 .First().Action == HistoryAction.Borrowed &&
                             bc.History
                                 .OrderByDescending(he => he.TimeStamp)
                                 .First().UserId == userId)
                .Select(bc => new BookCopyDto(
                    bc.Id,
                    bc.Book.Name,
                    bc.Book.Author,
                    bc.Book.Year,
                    bc.Book.Isbn,
                    bc.History
                        .OrderByDescending(he => he.TimeStamp)
                        .Where(he => he.UserId == userId)
                        .Select(he => he.User.Name)
                        .FirstOrDefault(),
                    false
                ));

        public IQueryable<HistoryEntryDto> GetHistoryEntriesByBookCopyQuery(Guid bookCopyId) =>
            context.HistoryEntries
                .Where(he => he.BookCopyId == bookCopyId)
                .OrderByDescending(he => he.TimeStamp)
                .Select(he => new HistoryEntryDto(
                    he.Id,
                    he.TimeStamp,
                    he.Action,
                    he.UserId
                ));
        
        public IQueryable<HistoryEntryDto> GetHistoryEntriesByUserQuery(Guid userId) =>
            context.HistoryEntries
                .Where(he => he.UserId == userId)
                .OrderByDescending(he => he.TimeStamp)
                .Select(he => new HistoryEntryDto(
                    he.Id,
                    he.TimeStamp,
                    he.Action,
                    he.UserId
                ));

        public async Task<List<BookDto>> GetBooksAsync() => await context.GetBooksQueryable().ToListAsync();

        public async Task<List<BookCopyDto>> GetBookCopiesByBookAsync(Guid bookId,
            Func<IQueryable<BookCopyDto>, IQueryable<BookCopyDto>>? restriction = null)
        {
            var query = context.GetBookCopiesByBookQueryable(bookId);

            if (restriction != null)
            {
                query = restriction(query);
            }

            return await query.AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<BookCopyDto>> GetBookCopiesByUserAsync(Guid userId) =>
            await context.GetBookCopiesByUserQueryable(userId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<HistoryEntryDto>> GetHistoryEntriesByBookCopyAsync(Guid bookCopyId) =>
            await context.GetHistoryEntriesByBookCopyQuery(bookCopyId)
                .AsNoTracking()
                .ToListAsync();
        
        public async Task<List<HistoryEntryDto>> GetHistoryEntriesByUserAsync(Guid userId) =>
            await context.GetHistoryEntriesByBookCopyQuery(userId)
                .AsNoTracking()
                .ToListAsync(); 

        public async Task<bool> BorrowBookCopyAsync(Guid bookCopyId, Guid userId)
        {
            var bookCopy = await context.BookCopies
                .Where(bc =>
                    bc.Id == bookCopyId &&
                    bc.IsAvailable &&
                    !bc.IsDeleted)
                .FirstOrDefaultAsync();

            if (bookCopy is null)
                return false;

            try
            {
                bookCopy.IsAvailable = false;

                context.HistoryEntries.Add(new HistoryEntry
                {
                    Id = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow,
                    Action = HistoryAction.Borrowed,
                    BookCopyId = bookCopyId,
                    UserId = userId
                });

                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ReturnBookCopyAsync(Guid bookCopyId, Guid userId)
        {
            var bookCopy = await context.BookCopies
                .Include(bc => bc.History)
                .FirstOrDefaultAsync(bc => bc.Id == bookCopyId && !bc.IsDeleted);

            if (bookCopy is null || bookCopy.IsAvailable)
                return false;

            var last = bookCopy.History
                .OrderByDescending(h => h.TimeStamp)
                .FirstOrDefault();

            if (last is null ||
                last.Action != HistoryAction.Borrowed ||
                last.UserId != userId)
                return false;

            try
            {
                bookCopy.IsAvailable = true;

                context.HistoryEntries.Add(new HistoryEntry
                {
                    Id = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow,
                    Action = HistoryAction.Returned,
                    BookCopyId = bookCopyId,
                    UserId = userId
                });

                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DoesUserExistAsync(Guid userId) =>
            await context.Users.AnyAsync(u => u.Id == userId);

        public async Task<UserDto?> GetUserAsync(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is not null)
            {
                return new UserDto(user.Id, user.Name, user.Email);
            }

            return null;
        }

        public async Task<UserDto> GetOrRegisterUserAsync(string email, string name)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is not null)
            {
                return new UserDto(user.Id, user.Name, user.Email);
            }

            var result = await context.Users.AddAsync(new User()
            {
                Email = email,
                Name = name
            });

            await context.SaveChangesAsync();
            return new UserDto(result.Entity.Id, result.Entity.Name, result.Entity.Email);
        }

        public async Task<bool> AddBookAsync(string name, string author, int year, string isbn)
        {
            var book = new Book()
            {
                Name = name,
                Author = author,
                Year = year,
                Isbn = isbn
            };
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddBookCopyAsync(Guid bookId, Guid userId)
        {
            if (!await context.Books.AnyAsync(b => b.Id == bookId))
            {
                return false;
            }

            if (!await context.DoesUserExistAsync(userId))
            {
                return false;
            }

            await context.BookCopies.AddAsync(new BookCopy()
            {
                BookId = bookId,
                IsAvailable = true,
                History =
                [
                    new()
                    {
                        Action = HistoryAction.Registered,
                        TimeStamp = DateTime.UtcNow,
                        UserId = userId
                    }
                ]
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<BookDto?> GetBookAsync(Guid bookId) =>
            await context.Books
                .Where(b => !b.IsDeleted && b.Id == bookId)
                .Select(b => new BookDto(
                    b.Id,
                    b.Name,
                    b.Author,
                    b.Year,
                    b.Isbn,
                    b.Copies.Count(c => c.IsAvailable)
                ))
                .FirstOrDefaultAsync();

        public async Task<bool> RemoveBookAsync(Guid bookId)
        {
            var book = await context.Books.FirstOrDefaultAsync(book => book.Id == bookId);

            if (book is null)
            {
                return false;
            }

            try
            {
                book.IsDeleted = true;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveBookCopyAsync(Guid bookCopyId)
        {
            var bookCopy = await context.BookCopies.FirstOrDefaultAsync(copy => copy.Id == bookCopyId);

            if (bookCopy is null)
            {
                return false;
            }

            try
            {
                bookCopy.IsDeleted = true;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }
    }
}