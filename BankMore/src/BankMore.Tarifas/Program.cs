using BankMore.Tarifas;
using BankMore.Tarifas.Data;
using BankMore.Tarifas.Handlers;
using BankMore.Tarifas.Services;
using KafkaFlow;
using Serilog;
using Serilog.Events;

// ===== Configuração do Serilog (Logging Estruturado) =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("KafkaFlow", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "BankMore.Tarifas")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Iniciando BankMore.Tarifas Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // Adiciona Serilog
    builder.Services.AddSerilog();

// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=tarifas.db";

// Registrar serviços
builder.Services.AddSingleton<ITarifaRepository>(sp => 
    new TarifaRepository(connectionString));

builder.Services.AddHttpClient<IContaCorrenteService, ContaCorrenteService>();

// Configurar KafkaFlow Consumer
builder.Services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092" })
        .AddConsumer(consumer => consumer
            .Topic("transferencias-realizadas")
            .WithGroupId("tarifas-consumer-group")
            .WithBufferSize(10)
            .WithWorkersCount(2)
            .AddMiddlewares(middlewares => middlewares
                .AddTypedHandlers(handlers => handlers
                    .AddHandler<TransferenciaConsumerHandler>()
                )
            )
        )
    )
);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Inicializar banco de dados
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();
var dbInitializer = new DatabaseInitializer(connectionString, loggerFactory.CreateLogger<DatabaseInitializer>());
await dbInitializer.InitializeAsync();

// Iniciar o KafkaFlow bus
var kafkaBus = host.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

    logger.LogInformation("Serviço de Tarifas iniciado. Consumindo tópico 'transferencias-realizadas'");

    host.Run();

    await kafkaBus.StopAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker encerrado inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
