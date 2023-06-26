using KafkaFlow;
using LetsTalk.Server.API;
using LetsTalk.Server.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.AddApiServices(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseCustomExceptionHandling();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors("all");

app.UseRouting();

app.UseJwtMiddleware();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Run();