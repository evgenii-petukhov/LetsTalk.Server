using LetsTalk.Server.Authentication;
using JwtTokenGrpcService = LetsTalk.Server.Authentication.Services.JwtTokenGrpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddAuthenticationServices(builder.Configuration);

var app = builder.Build();

app.UseCors("all");

app.MapGrpcReflectionService();

app.MapGrpcService<JwtTokenGrpcService>()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
