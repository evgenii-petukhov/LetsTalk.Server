using LetsTalk.Server.Notifications;
using LetsTalk.Server.Notifications.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddNotificationsServices(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseCors("all");

app.UseSerilogRequestLogging();

app.MapHub<NotificationHub>("/messagehub");

app.Run();
