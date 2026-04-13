using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Torisho.Application;
using Torisho.Application.Interfaces.Auth;
using Torisho.Application.Interfaces.Room;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Application.Interfaces.Learning;
using Torisho.Application.Services.Dictionary;
using Torisho.Application.Services.Learning;
using Torisho.Domain.Interfaces;
using Torisho.Domain.Interfaces.Repositories;
using Torisho.Infrastructure;
using Torisho.Infrastructure.Repositories;
using Torisho.Infrastructure.Services.Auth;
using Torisho.Infrastructure.Services.Room;
using Torisho.API.Hubs;
using Torisho.Infrastructure.Services.Dictionary;
using Torisho.Infrastructure.Services.Learning;
using Torisho.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

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
            mySqlOptions.CommandTimeout(30);
            mySqlOptions.MigrationsAssembly("Torisho.Infrastructure");
        }
    );

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddScoped<IDataContext>(provider => 
    provider.GetRequiredService<DataContext>());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IJmdictImportService, JmdictImportService>();
builder.Services.AddScoped<ICurriculumImportService, CurriculumImportService>();
builder.Services.AddScoped<ILearningQueryService, LearningQueryService>();
builder.Services.AddScoped<IDictionarySearchService, DictionarySearchService>();
builder.Services.AddScoped<IDictionaryDetailService, DictionaryDetailService>();
builder.Services.AddScoped<IDictionaryCommentService, DictionaryCommentService>();
builder.Services.AddScoped<IDictionaryEntryRepository, DictionaryEntryRepository>();
builder.Services.AddScoped<IDictionaryKanjiRepository, DictionaryKanjiRepository>();
builder.Services.AddScoped<IDictionaryKanjiService, DictionaryKanjiService>();
builder.Services.AddHttpClient<ITatoeba, TatoebaService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // SignalR authentication via query string token
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// SignalR for real-time room communication
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<RoomHub>("/hubs/room");

app.Run();
