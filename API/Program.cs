using API.Extensions;
using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityService(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddCors();

// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// builder.Logging.SetMinimumLevel(LogLevel.Debug); // 👈 Add this line


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();

        await context.Database.MigrateAsync();
        await Seeder.SeedUsers(userManager, roleManager, unitOfWork);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occured while seeding database");
    }
}

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("https://localhost:4200")
);

app.UseMiddleware<RequestLocalizationMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<PresenceHub>("hubs/presence");

app.MapHub<TwoPersonsMessageHub>("hubs/dual-messaging");

app.MapControllers();

app.Run();
