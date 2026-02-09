using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace LibraryApplication.Extensions;

public static class AuthenticationServiceCollectionExtensions
{
    public static WebApplicationBuilder AddGoogleAuthentication(this WebApplicationBuilder builder)
    {
        var authenticationBuilder = builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/signin-google";
                options.LogoutPath = "/debugLogout";
            });
            
        if (!string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Google:ClientId"]))
        {
            authenticationBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;

                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                                       ?? throw new ApplicationException("Missing Authentication:Google:ClientSecret");

                options.Events.OnCreatingTicket = context =>
                {
                    var identity = (ClaimsIdentity)context.Principal!.Identity!;
                    identity.AddClaim(new Claim("auth_provider", "google"));
                    return Task.CompletedTask;
                };
            });
        }
        
        builder.Services.AddAuthorization();
        
        return builder;
    }
}