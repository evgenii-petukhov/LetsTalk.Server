using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Notifications;
using LetsTalk.Server.Notifications.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);

var kafkaUrl = builder.Configuration.GetValue<string>("Kafka:Url");
var messageNotificationTopic = builder.Configuration.GetValue<string>("Kafka:MessageNotificationTopic");
var messageNotificationGroupId = builder.Configuration.GetValue<string>("Kafka:MessageNotificationGroupId");

builder.Services.AddNotificationsServices();
builder.Services.AddAuthenticationClientServices();
builder.Services.AddConfigurationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});
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

var app = builder.Build();

app.UseCors("all");

app.MapHub<NotificationHub>("/messagehub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Run();
