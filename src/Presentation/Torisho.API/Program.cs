using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Torisho.Application;
using Torisho.Application.Services;
using Torisho.Infrastructure;
using Torisho.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// add DbContext 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );

            // Command timeout
            mySqlOptions.CommandTimeout(30);

            mySqlOptions.MigrationsAssembly("Torisho.Infrastructure");
        }
    );

    // Enable sensitive data logging (Development only)
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register IDataContext for Application layer
builder.Services.AddScoped<IDataContext>(provider => 
    provider.GetRequiredService<DataContext>());

// Configure Redis
var redisConfig = builder.Configuration.GetSection("Redis");
var redisHost = redisConfig["Host"];
var redisPort = redisConfig["Port"];
var redisPassword = redisConfig["Password"];
var redisDatabase = int.Parse(redisConfig["Database"] ?? "0");

var redisConnectionString = $"{redisHost}:{redisPort},password={redisPassword},abortConnect=false";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.ConnectTimeout = int.Parse(redisConfig["ConnectTimeout"] ?? "5000");
    configuration.SyncTimeout = int.Parse(redisConfig["SyncTimeout"] ?? "5000");
    configuration.AbortOnConnectFail = bool.Parse(redisConfig["AbortOnConnectFail"] ?? "false");
    
    return ConnectionMultiplexer.Connect(configuration);
});

// Register Redis Service
builder.Services.AddScoped<IRedisService, RedisService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Weather forecast endpoint
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
