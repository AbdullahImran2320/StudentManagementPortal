using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models ;
using StudentAPI.Data;
using StudentAPI.Interfaces;
using StudentAPI.Middleware;
using StudentAPI.Models;
using StudentAPI.Repositories;
using StudentAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));

// ── JWT SETTINGS ──────────────────────────────
var secretKey = builder.Configuration[
    "JwtSettings:SecretKey"]!;
var issuer = builder.Configuration[
    "JwtSettings:Issuer"]!;
var audience = builder.Configuration[
    "JwtSettings:Audience"]!;
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

// ── DATABASE ──────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration
               .GetConnectionString("DefaultConnection")
    )
);

// ── CORS ──────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ── REPOSITORIES & SERVICES ───────────────────
builder.Services.AddScoped<IStudentRepository,
                            StudentRepositories>();
builder.Services.AddScoped<IAuthService,
                            AuthService>();
builder.Services.AddScoped<IEmailService,
                            EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();


// ── JWT AUTHENTICATION ────────────────────────
builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// ── SWAGGER ───────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token!"
        });
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});


// ── BUILD ─────────────────────────────────────
var app = builder.Build();

// ── AUTO-APPLY MIGRATIONS ─────────────────────
// So the packaged desktop installer works on a machine that has
// never run `dotnet ef database update`.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup] Database migration failed: {ex.Message}");
    }
}

// ── PIPELINE ──────────────────────────────────
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Serve the Angular production build placed in wwwroot by build.bat
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    // SPA fallback: any route that isn't an API route or a static file
    // serves index.html so Angular's router can handle it.
    app.MapFallbackToFile("index.html");
}

app.Run();