using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Asp.Versioning;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Producers;
using StackExchange.Redis;
using BankMore.Transferencia.Application.Handlers;
using BankMore.Transferencia.Domain.Interfaces;
using BankMore.Transferencia.Infrastructure.Repositories;
using BankMore.Transferencia.Infrastructure.Services;
using BankMore.Transferencia.Infrastructure.Messaging;
using Serilog;
using Serilog.Events;
using Prometheus;

// ===== Configuração do Serilog (Logging Estruturado) =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "BankMore.Transferencia")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Iniciando BankMore.Transferencia API");

    var builder = WebApplication.CreateBuilder(args);

    // Adiciona Serilog
    builder.Host.UseSerilog();

// Configuração JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey não configurada");

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

builder.Services.AddAuthorization();

// API Versioning (RESTful)
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

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RealizarTransferenciaHandler).Assembly));

// Repositories (Dapper)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<ITransferenciaRepository>(sp => new TransferenciaRepository(connectionString!));
builder.Services.AddScoped<ICompensacaoPendenteRepository>(sp => new CompensacaoPendenteRepository(connectionString!));
builder.Services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(connectionString!));

// ===== Configuração do Outbox Pattern =====
builder.Services.AddScoped<BankMore.Transferencia.Domain.Interfaces.IOutboxEventRepository, 
    BankMore.Transferencia.Infrastructure.Repositories.OutboxEventRepository>();
builder.Services.AddSingleton<BankMore.Transferencia.Domain.Interfaces.IMessagePublisher, 
    BankMore.Transferencia.Infrastructure.Messaging.KafkaMessagePublisher>();
builder.Services.AddHostedService<BankMore.Transferencia.Infrastructure.Messaging.OutboxProcessor>();

// Redis para Idempotência
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
var idempotencyRetentionHours = int.Parse(builder.Configuration["Idempotency:RetentionHours"] ?? "24");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<IIdempotenciaRepository>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    var ttl = TimeSpan.FromHours(idempotencyRetentionHours);
    return new IdempotenciaRepository(redis, ttl);
});

// HttpClient para API Conta Corrente
var apiContaUrl = builder.Configuration.GetSection("ApiContaCorrente")["BaseUrl"] 
    ?? throw new InvalidOperationException("BaseUrl da API Conta não configurada");

builder.Services.AddHttpClient<IContaCorrenteService, ContaCorrenteService>((client) =>
{
    client.BaseAddress = new Uri(apiContaUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    return new ContaCorrenteService(client, apiContaUrl);
});

// Kafka Producer (COMENTADO TEMPORARIAMENTE PARA TESTES SEM BROKER)
// var kafkaBootstrapServers = builder.Configuration.GetSection("Kafka")["BootstrapServers"] 
//     ?? "localhost:9092";

// builder.Services.AddKafka(kafka => kafka
//     .AddCluster(cluster => cluster
//         .WithBrokers(new[] { kafkaBootstrapServers })
//         .CreateTopicIfNotExists("transferencias-realizadas", 1, 1)
//         .AddProducer(
//             "transferencia-producer",
//             producer => producer
//                 .DefaultTopic("transferencias-realizadas")
//         )
//     )
// );

// Kafka Producer Service (Mock para testes sem Kafka)
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerServiceMock>();

// Valor da tarifa (configurável) - inject como factory
var valorTarifa = decimal.Parse(builder.Configuration.GetSection("Tarifa")["ValorTransferencia"] ?? "2.00");
builder.Services.AddTransient(sp => new RealizarTransferenciaHandler(
    sp.GetRequiredService<ITransferenciaRepository>(),
    sp.GetRequiredService<IIdempotenciaRepository>(),
    sp.GetRequiredService<ICompensacaoPendenteRepository>(),
    sp.GetRequiredService<IContaCorrenteService>(),
    sp.GetRequiredService<IKafkaProducerService>(),
    sp.GetRequiredService<IConfiguration>()
));

builder.Services.AddControllers();

// ===== Configuração de Health Checks =====
var redisConnString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=transferencia.db", 
        name: "sqlite",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "db", "ready" })
    .AddRedis(redisConnString,
        name: "redis",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "cache", "ready" })
    .AddKafka(new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
    },
        name: "kafka",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "messaging", "ready" });

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BankMore - API Transferência",
        Version = "1.0.0",
        Description = @"API RESTful para transferências entre contas correntes.

**Versão:** 1.0.0  
**Build:** " + Assembly.GetExecutingAssembly().GetName().Version + @"  
**Framework:** .NET 9.0

Padrões implementados: Versioning, HATEOAS, Problem Details (RFC 7807), Paginação, Idempotência.

**Features:**
- Transferências com validação de saldo
- Compensação automática com retry
- Idempotência via Redis (24h TTL)
- Publicação de eventos no Kafka
- DLQ para transferências falhadas
- Outbox Pattern para consistência",
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

    // JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Exemplo: 'Bearer {token}'",
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

    // XML Documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Criar banco de dados SQLite
await InicializarBancoDeDados(connectionString!);

// Swagger em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Transferência v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});

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

static async Task InicializarBancoDeDados(string connectionString)
{
    var dbPath = connectionString.Replace("Data Source=", "");
    
    if (!File.Exists(dbPath))
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        await connection.OpenAsync();

        // Tabela transferencia
        var createTableTransferencia = @"
            CREATE TABLE IF NOT EXISTS transferencia (
                id TEXT(37) PRIMARY KEY,
                idcontacorrenteorigem TEXT(37) NOT NULL,
                idcontacorrentedestino TEXT(37) NOT NULL,
                valor REAL NOT NULL,
                datamovimento TEXT(25) NOT NULL
            );";

        // Tabela idempotencia
        var createTableIdempotencia = @"
            CREATE TABLE IF NOT EXISTS idempotencia (
                chave_idempotencia TEXT(100) PRIMARY KEY,
                requisicao TEXT NOT NULL,
                resultado TEXT NOT NULL,
                data_criacao TEXT(25) NOT NULL
            );";

        // Tabela outbox_events
        var createTableOutbox = @"
            CREATE TABLE IF NOT EXISTS outbox_events (
                id TEXT(37) PRIMARY KEY,
                topic TEXT(100) NOT NULL,
                event_type TEXT(100) NOT NULL,
                payload TEXT NOT NULL,
                partition_key TEXT(100),
                created_at TEXT(25) NOT NULL,
                processed_at TEXT(25),
                processed INTEGER(1) DEFAULT 0,
                retry_count INTEGER DEFAULT 0,
                error_message TEXT(500)
            );
            
            CREATE INDEX IF NOT EXISTS IX_outbox_events_processed_created_at ON outbox_events(processed, created_at);
            CREATE INDEX IF NOT EXISTS IX_outbox_events_topic ON outbox_events(topic);";

        using var cmdTransferencia = new Microsoft.Data.Sqlite.SqliteCommand(createTableTransferencia, connection);
        await cmdTransferencia.ExecuteNonQueryAsync();

        using var cmdIdempotencia = new Microsoft.Data.Sqlite.SqliteCommand(createTableIdempotencia, connection);
        await cmdIdempotencia.ExecuteNonQueryAsync();

        using var cmdOutbox = new Microsoft.Data.Sqlite.SqliteCommand(createTableOutbox, connection);
        await cmdOutbox.ExecuteNonQueryAsync();

        Console.WriteLine($"Banco de dados criado: {dbPath}");
    }
}
