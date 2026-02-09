using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace LibraryApplication.Extensions;

public static class AuthenticationWebApplicationExtensions
{
    public static WebApplication MapGoogleAuthentication(this WebApplication app)
    {
        app.MapGet("/login", async (HttpContext context) =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/" // Redirect to root after login
            };
            await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        });

        app.MapPost("/logout", (HttpContext context) =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/" // Redirect to root after login
            };
            return Results.SignOut(properties, ["Cookies"]);
        });
        
        return app;
    }
}