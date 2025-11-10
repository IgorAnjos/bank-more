using System.Text;
using Asp.Versioning;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using BankMore.ContaCorrente.Application.Services;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Prometheus;

// ===== Configuração do Serilog (Logging Estruturado) =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "BankMore.ContaCorrente")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Iniciando BankMore.ContaCorrente API");

    var builder = WebApplication.CreateBuilder(args);

    // Adiciona Serilog
    builder.Host.UseSerilog();

// ===== Configuração do SQLite =====
builder.Services.AddDbContext<ContaCorrenteDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("BankMore.ContaCorrente.Api")
    ));

// ===== Configuração de Repositórios (Repository Pattern) =====
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ===== Configuração do Outbox Pattern =====
builder.Services.AddScoped<BankMore.ContaCorrente.Domain.Interfaces.IOutboxEventRepository, 
    BankMore.ContaCorrente.Infrastructure.Repositories.OutboxEventRepository>();
builder.Services.AddSingleton<BankMore.ContaCorrente.Domain.Interfaces.IMessagePublisher, 
    BankMore.ContaCorrente.Infrastructure.Messaging.KafkaMessagePublisher>();
builder.Services.AddHostedService<BankMore.ContaCorrente.Infrastructure.Messaging.OutboxProcessor>();

// ===== Configuração do Redis =====
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
var redisInstanceName = builder.Configuration["Redis:InstanceName"] ?? "BankMore:ContaCorrente:";
var idempotencyRetentionHours = int.Parse(builder.Configuration["Idempotency:RetentionHours"] ?? "24");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<IIdempotenciaRepository>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    var ttl = TimeSpan.FromHours(idempotencyRetentionHours);
    return new IdempotenciaRepository(redis, ttl);
});

// ===== Configuração do Serviço de Criptografia =====
var encryptionKey = builder.Configuration["EncryptionKey"] ?? "BankMore-Secret-Key-2024-Minimum32Characters!";
builder.Services.AddSingleton<BankMore.ContaCorrente.Infrastructure.Services.ICryptographyService>(
    sp => new BankMore.ContaCorrente.Infrastructure.Services.CryptographyService(encryptionKey));

// ===== Configuração do JWT Service =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddSingleton<IJwtService>(sp => 
    new JwtService(
        jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada"),
        jwtSettings["Issuer"] ?? "BankMore",
        jwtSettings["Audience"] ?? "BankMore",
        int.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "10"),
        int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "1")
    ));

// ===== Configuração do MediatR (padrão Ailos) =====
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(BankMore.ContaCorrente.Application.Commands.CadastrarContaCommand).Assembly);
});

// ===== Configuração de Autenticação JWT =====
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada"))
        )
    };
});

builder.Services.AddAuthorization();

// ===== Configuração de Versionamento de API =====
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ===== Configuração de Controllers =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // PascalCase
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ===== Configuração do Swagger (padrão Ailos - documentação completa) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BankMore - API Conta Corrente",
        Version = "1.0.0",
        Description = @"API para gerenciamento de contas correntes e movimentações. 
                        
**Versão:** 1.0.0  
**Build:** " + Assembly.GetExecutingAssembly().GetName().Version + @"  
**Framework:** .NET 9.0  

Desenvolvida seguindo padrões DDD, CQRS, MediatR e Outbox Pattern.

**Features:**
- Autenticação JWT (Access + Refresh Token)
- Idempotência via Redis (24h TTL)
- Criptografia AES-256 para dados sensíveis
- Outbox Pattern para eventos Kafka
- Versionamento de API",
        Contact = new OpenApiContact
        {
            Name = "BankMore Team",
            Email = "contato@bankmore.com.br",
            Url = new Uri("https://github.com/bankmore")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Configuração de autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ===== Configuração de Health Checks =====
builder.Services.AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=contacorrente.db", 
        name: "sqlite",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "db", "ready" })
    .AddRedis(redisConnectionString,
        name: "redis",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "cache", "ready" })
    .AddKafka(new Confluent.Kafka.ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
    },
        name: "kafka",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "messaging", "ready" });

// ===== Configuração de CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===== Aplicar Migrations automaticamente =====
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContaCorrenteDbContext>();
    dbContext.Database.EnsureCreated(); // Cria o banco se não existir
}

// ===== Configuração do Pipeline HTTP =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Conta Corrente v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// ===== Health Checks Endpoints =====
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // Apenas verifica se a aplicação está rodando
});

// ===== Prometheus Metrics =====
app.UseHttpMetrics(); // Métricas HTTP automáticas
app.MapMetrics();      // Endpoint /metrics

app.MapControllers();

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
