var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

var app = builder.Build();

app.UseCors("all");

app.UseGrpcWeb();

app.MapGrpcReflectionService();

app.Run();
