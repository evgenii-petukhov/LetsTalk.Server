using LetsTalk.Server.FileStorage.Service;
using LetsTalk.Server.FileStorage.Service.GrpcEndpoints;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddFileStorageServices(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
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

app.Run();
