using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text;
using System.Net.Http.Headers;
using Torisho.Application;
using Torisho.Application.Auth;
using Torisho.Application.Interfaces.Auth;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Application.Interfaces.Room;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Application.Interfaces.Learning;
using Torisho.Application.Interfaces.Quiz;
using Torisho.Application.Interfaces.Email;
using Torisho.Application.Services.Flashcard;
using Torisho.Application.Services.Dictionary;
using Torisho.Application.Services.Learning;
using Torisho.Application.Services.Quiz;
using Torisho.Domain.Interfaces;
using Torisho.Domain.Interfaces.Repositories;
using Torisho.Infrastructure;
using Torisho.Infrastructure.Repositories;
using Torisho.Infrastructure.Services.Auth;
using Torisho.Infrastructure.Services.Room;
using Torisho.Infrastructure.Services.Email;
using Torisho.API.Hubs;
using Torisho.Application.Validators.Auth;
using Torisho.Infrastructure.Services.Dictionary;
using Torisho.Infrastructure.Services.Learning;
using Torisho.Infrastructure.ExternalServices;
using Torisho.Application.Interfaces.Dashboard;
using Torisho.Application.Services.Dashboard;
using Torisho.Infrastructure.Seed;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

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
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IExternalAuthProvider, GoogleAuthProvider>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IJmdictImportService, JmdictImportService>();
builder.Services.AddScoped<ICurriculumImportService, CurriculumImportService>();
builder.Services.AddScoped<ILearningQueryService, LearningQueryService>();
builder.Services.AddScoped<IPreparedQuizService, PreparedQuizService>();
builder.Services.AddScoped<IDailyQuizService, DailyQuizService>();
builder.Services.AddHttpClient<IQuizTemplateAiService, QuizTemplateAiService>();
builder.Services.AddScoped<IDictionarySearchService, DictionarySearchService>();
builder.Services.AddScoped<IDictionaryDetailService, DictionaryDetailService>();
builder.Services.AddScoped<IDictionaryCommentService, DictionaryCommentService>();
builder.Services.AddScoped<IFlashcardDeckService, FlashcardDeckService>();
builder.Services.AddScoped<IFlashcardFolderService, FlashcardFolderService>();
builder.Services.AddScoped<IFlashcardQueryService, FlashcardQueryService>();
builder.Services.AddScoped<IFlashcardStudyService, FlashcardStudyService>();
builder.Services.AddScoped<IDictionaryEntryRepository, DictionaryEntryRepository>();
builder.Services.AddScoped<IDictionaryKanjiRepository, DictionaryKanjiRepository>();
builder.Services.AddScoped<IDictionaryKanjiService, DictionaryKanjiService>();
builder.Services.AddHttpClient<IKanjiRecognitionClient, SljfaqKanjiRecognitionClient>(client =>
{
    client.BaseAddress = new Uri("https://kanji.sljfaq.org/");
    client.Timeout = TimeSpan.FromSeconds(8);
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddScoped<ILearningTrackingService, LearningTrackingService>();
builder.Services.AddScoped<IDashboardQueryService, DashboardQueryService>();
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
        policy.RequireRole(AppRoles.Admin));

    options.AddPolicy(AuthorizationPolicies.CanImportCurriculum, policy =>
        policy.RequireClaim("Permission", AppPermissions.CurriculumImport));

    options.AddPolicy(AuthorizationPolicies.CanManageQuiz, policy =>
        policy.RequireClaim("Permission", AppPermissions.QuizManage));

    options.AddPolicy(AuthorizationPolicies.CanModerateComments, policy =>
        policy.RequireClaim("Permission", AppPermissions.CommentsModerate));
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
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
        policy.WithOrigins(
                  "http://localhost:3000",
                  "http://localhost:5173",
                  "https://dhung.xyz",
                  "https://www.dhung.xyz",
                  "https://torisho-fe.vercel.app")
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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("AuthDataSeeder");

    await AuthDataSeeder.SeedAsync(context, passwordHasher, app.Configuration, logger);
}

app.Run();
