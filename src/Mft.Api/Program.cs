using Mft.Api.Authorization;
using Mft.Api.Middleware;
using Mft.Application.Common;
using Mft.Infrastructure;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMftInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    foreach (var (key, _, _) in PermissionKeys.Catalog)
    {
        options.AddPolicy(key, policy => policy.Requirements.Add(new PermissionRequirement(key)));
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaDev", policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MftDbContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("SpaDev");
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();

// Serves the built React SPA (see Mft.Api.csproj's BuildClientApp target) so the API and UI can
// share a single App Service/origin. wwwroot only exists after a publish, not `dotnet run` - the
// guard keeps local dev (where the Vite dev server is used instead) from registering a fallback
// route to a file that doesn't exist.
var indexHtmlPath = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html");
var servesSpa = File.Exists(indexHtmlPath);
if (servesSpa)
{
    app.UseStaticFiles();
}

app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

app.MapControllers();

if (servesSpa)
{
    app.MapFallbackToFile("index.html");
}

app.Run();
