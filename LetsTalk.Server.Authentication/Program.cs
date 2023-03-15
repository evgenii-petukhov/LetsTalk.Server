using LetsTalk.Server.Authentication.Services;

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

var app = builder.Build();
app.UseCors("all");
app.UseGrpcWeb();
app.MapGrpcReflectionService();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb()
    .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
