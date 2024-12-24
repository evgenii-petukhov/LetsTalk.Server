using LetsTalk.Server.Authentication;
using Serilog;
using System.Globalization;
using JwtTokenGrpcService = LetsTalk.Server.Authentication.Services.JwtTokenGrpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseCors("all");

app.UseSerilogRequestLogging();

app.MapGrpcReflectionService();

app.MapGrpcService<JwtTokenGrpcService>()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
