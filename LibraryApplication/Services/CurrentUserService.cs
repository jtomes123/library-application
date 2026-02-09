using LibraryApplication.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using LibraryApplication.Extensions;

namespace LibraryApplication.Services;

public class CurrentUserService
{
    const string EmailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    
    AuthenticationStateProvider AuthenticationStateProvider { get; }
    IDbContextFactory<LibraryDbContext> DbContextFactory { get; }

    public CurrentUserService(AuthenticationStateProvider authenticationStateProvider, IDbContextFactory<LibraryDbContext> dbContextFactory)
    {
        AuthenticationStateProvider = authenticationStateProvider;
        DbContextFactory = dbContextFactory;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var emailClaim = authenticationState.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaimType);
        var nameClaim = authenticationState.User.Claims.FirstOrDefault(claim => claim.Type == NameClaimType);

        if (emailClaim is null || nameClaim is null)
        {
            return null;
        }
        
        return await context.GetOrRegisterUserAsync(emailClaim.Value, nameClaim.Value);
    }
}