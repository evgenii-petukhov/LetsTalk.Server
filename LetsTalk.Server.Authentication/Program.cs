using LetsTalk.Server.Authentication.Services;
using LetsTalk.Server.Identity;

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
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseCors("all");

app.UseGrpcWeb();

app.MapGrpcReflectionService();

app.MapGrpcService<JwtTokenService>()
    .EnableGrpcWeb()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
