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

app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

app.MapControllers();

app.Run();
