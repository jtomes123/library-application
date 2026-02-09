using LibraryApplication.Services;
using Microsoft.AspNetCore.SignalR;

namespace LibraryApplication.Extensions;

public static class ApiExtensions
{
    public static WebApplication MapLibraryApi(this WebApplication app)
    {
        app.MapGet("api/books", async (LibraryDbContext context) => Results.Ok((object?)await context.GetBooksAsync()))
            .DisableAntiforgery().AllowAnonymous();

        app.MapGet("api/books/{bookId}/copies", async (Guid bookId, LibraryDbContext context) =>
        {
            var copies = await context.GetBookCopiesByBookAsync(bookId);
            
            return Results.Ok(copies);
        }).DisableAntiforgery().AllowAnonymous();

        app.MapGet("api/users/{userId}/books", async (Guid userId, LibraryDbContext context) =>
        {
            var copies = await context.GetBookCopiesByUserAsync(userId);
            
            return Results.Ok(copies);
        }).DisableAntiforgery().AllowAnonymous();
        

        app.MapPost("api/users/{userId}/borrow/byCopyId/{copyId}", async (Guid userId, Guid copyId, LibraryDbContext context) =>
        {
            if (!await context.DoesUserExistAsync(userId))
            {
                return Results.BadRequest("User not found");
            }
            
            var result = await context.BorrowBookCopyAsync(copyId, userId);
            
            return result ? Results.Ok() : Results.BadRequest("Failed to borrow book copy");
        }).DisableAntiforgery().AllowAnonymous();
        
        app.MapPost("api/users/{userId}/return/byCopyId/{copyId}",
            async (Guid userId, Guid copyId, LibraryDbContext context) =>
            {
                if (!await context.DoesUserExistAsync(userId))
                {
                    return Results.BadRequest("User not found");
                }
            
                var result = await context.ReturnBookCopyAsync(copyId, userId);
            
                return result ? Results.Ok() : Results.BadRequest("Failed to return book copy");
            }).DisableAntiforgery().AllowAnonymous();
        
        return app;
    }
}