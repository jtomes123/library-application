using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LibraryApplication.Extensions;

public static class DebugAuthExtensions
{
    public static WebApplication MapDebugAuth(this WebApplication app)
    {
        if (app.Configuration["Authentication:Debug"] is null)
            return app;

        app.MapGet("/debugLogin/{role}", async (
                string role,
                HttpContext httpContext,
                IWebHostEnvironment env) =>
            {
                if (!env.IsDevelopment())
                    return Results.NotFound();

                role = role.Equals("admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : "User";

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, $"debug-{role.ToLower()}"),
                    new(ClaimTypes.Name, $"Debug {role}"),
                    new(ClaimTypes.Role, role),
                    new(ClaimTypes.Email, $"{role}@debug.lan"),
                    new("auth_provider", "debug")
                };

                var principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return Results.Redirect("/");
            })
            .AllowAnonymous();

        app.MapGet("/debugLogout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Redirect("/");
        });

        return app;
    }
}