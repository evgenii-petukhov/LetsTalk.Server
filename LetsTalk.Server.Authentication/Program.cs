using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Services;
using LetsTalk.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddConfigurationServices(builder.Configuration);
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

var app = builder.Build();

app.UseCors("all");

app.UseGrpcWeb();

app.MapGrpcReflectionService();

app.MapGrpcService<JwtTokenGrpcService>()
    .EnableGrpcWeb()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
