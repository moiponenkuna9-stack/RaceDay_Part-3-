using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RaceDay.Data;
using RaceDay.Infrastructure;
using RaceDay.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RaceDay REST API", Version = "v1", Description = "Part 2 implementation of the RaceDay event management system." });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RaceDay.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RaceDayConnection")));

builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay REST API v1"));

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseSession();
app.UseMiddleware<SessionPrincipalMiddleware>();

app.MapControllers();

// This endpoint helps diagnose a missing SQL Server installation without crashing Swagger.
app.MapGet("/api/system/database-status", async (RaceDayDbContext db) =>
{
    try
    {
        var connected = await db.Database.CanConnectAsync();
        return connected
            ? Results.Ok(new { database = "connected", message = "RaceDay SQL Server is available." })
            : Results.Json(new { database = "unavailable", message = "SQL Server cannot be reached. See docs/LOCAL_SETUP_FIRST.md and appsettings.json." }, statusCode: 503);
    }
    catch (Exception ex)
    {
        return Results.Json(new { database = "unavailable", message = "Check SQL Server LocalDB installation and connection string.", detail = ex.GetBaseException().Message }, statusCode: 503);
    }
}).WithTags("System").WithName("DatabaseStatus");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    await DbInitializer.InitializeAsync(db, passwordService, scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer"));
}

app.Run();

/// <summary>
/// Provides the application entry point for integration testing.
/// </summary>
public partial class Program { }