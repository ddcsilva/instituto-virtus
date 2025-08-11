using InstitutoVirtus.API.HealthChecks;
using InstitutoVirtus.API.Middleware;
using InstitutoVirtus.API.Services;
using InstitutoVirtus.Application;
using InstitutoVirtus.Application.Common.Interfaces;
using InstitutoVirtus.Infrastructure;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using InstitutoVirtus.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/virtus-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNamingPolicy = null;
      options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Instituto Virtus API",
    Version = "v1",
    Description = "API para gestão acadêmica e financeira do Instituto Virtus",
    Contact = new OpenApiContact
    {
      Name = "Instituto Virtus",
      Email = "contato@institutovirtus.com.br"
    }
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "JWT Authorization header usando o esquema Bearer. Digite 'Bearer' [espaço] e então seu token.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada");

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
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
  };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
  options.AddPolicy("Coordenacao", policy => policy.RequireRole("Admin", "Coordenador"));
  options.AddPolicy("Professor", policy => policy.RequireRole("Admin", "Coordenador", "Professor"));
  options.AddPolicy("ResponsavelAluno", policy => policy.RequireRole("Admin", "Coordenador", "Responsavel", "Aluno"));
});

// CORS - usando extensão personalizada
builder.Services.AddCustomCors(builder.Configuration);

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
  options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
      RateLimitPartition.GetFixedWindowLimiter(
          partitionKey: httpContext.User?.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
          factory: partition => new FixedWindowRateLimiterOptions
          {
            AutoReplenishment = true,
            PermitLimit = 100,
            QueueLimit = 0,
            Window = TimeSpan.FromMinutes(1)
          }));

  options.OnRejected = async (context, token) =>
  {
    context.HttpContext.Response.StatusCode = 429;
    await context.HttpContext.Response.WriteAsync("Muitas requisições. Tente novamente mais tarde.", cancellationToken: token);
  };
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<VirtusDbContext>()
    .AddCheck<StorageHealthCheck>("Storage");

// Application services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Custom services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.ReportApiVersions = true;
});

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Instituto Virtus API V1");
  c.RoutePrefix = "swagger"; // Para acessar o Swagger em /swagger
});

if (!app.Environment.IsDevelopment())
{
  app.UseHsts();
  app.UseHttpsRedirection(); // Só redireciona para HTTPS em produção
}
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("DefaultPolicy");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Aplicar migrations automaticamente
await app.ApplyMigrationsAsync();

// Seed initial data
await app.SeedDataAsync();

Log.Information("API Instituto Virtus iniciada");
await app.RunAsync();
