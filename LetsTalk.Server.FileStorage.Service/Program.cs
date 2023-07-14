using KafkaFlow;
using LetsTalk.Server.FileStorage.Service;
using LetsTalk.Server.FileStorage.Service.GrpcEndpoints;
using LetsTalk.Server.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddFileStorageServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseCors("all");

app.UseSerilogRequestLogging();

app.UseGrpcWeb();

app.MapGrpcReflectionService();

app.MapGrpcService<FileUploadGrpcEndpoint>()
    .EnableGrpcWeb()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Run();
