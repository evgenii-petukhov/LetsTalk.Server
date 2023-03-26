using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.API;
using LetsTalk.Server.API.Middleware;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Core;
using LetsTalk.Server.Infrastructure;
using LetsTalk.Server.Persistence;
using LetsTalk.Server.SignalR;
using LetsTalk.Server.SignalR.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var confiruration = builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

var kafkaUrl = builder.Configuration.GetValue<string>("Kafka:Url");
var messageNotificationTopic = builder.Configuration.GetValue<string>("Kafka:MessageNotificationTopic");
var messageNotificationProducer = builder.Configuration.GetValue<string>("Kafka:MessageNotificationProducer");
var messageNotificationGroupId = builder.Configuration.GetValue<string>("Kafka:MessageNotificationGroupId");

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddLoggingServices();
builder.Services.AddSignalrServices();
builder.Services.AddAuthenticationClientServices();
builder.Services.AddSignalR();
builder.Services.AddControllers();
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
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });
});
builder.Services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[]
                {
                    kafkaUrl
                })
                .CreateTopicIfNotExists(messageNotificationTopic, 1, 1)
                .AddProducer(
                    messageNotificationProducer,
                    producer => producer
                        .DefaultTopic(messageNotificationTopic)
                        .AddMiddlewares(m =>
                            m.AddSerializer<JsonCoreSerializer>()
                        )
                )
                .AddConsumer(consumer => consumer
                    .Topic(messageNotificationTopic)
                    .WithGroupId(messageNotificationGroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<JsonCoreSerializer>()
                        .AddTypedHandlers(h => h.AddHandler<MessageNotificationHandler>())
                    )
                )
        )
);
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

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

app.MapHub<NotificationHub>("/messagehub");

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Run();