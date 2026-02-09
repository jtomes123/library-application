using Microsoft.EntityFrameworkCore;
using LibraryApplication.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Scalar.AspNetCore;
using LibraryApplication.Extensions;
using LibraryApplication.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SQLite");
builder.Services.AddDbContextFactory<LibraryDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.AddGoogleAuthentication();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGoogleAuthentication();
app.MapDebugAuth();

app.MapLibraryApi();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<LibraryDbContext>>();

    using var db = dbContextFactory.CreateDbContext();
    db.Database.Migrate();
}

app.Run();