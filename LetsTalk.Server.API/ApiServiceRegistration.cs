﻿using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Core;
using LetsTalk.Server.Infrastructure;

namespace LetsTalk.Server.API;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddCoreServices();
        services.AddPersistenceServices(configuration);
        services.AddLoggingServices();
        services.AddAuthenticationClientServices();
        services.AddConfigurationServices(configuration);
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("all", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddSwaggerGen(c =>
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

        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                    kafkaSettings.Url
                        })
                        .CreateTopicIfNotExists(kafkaSettings.MessageNotification!.Topic, 1, 1)
                        .CreateTopicIfNotExists(kafkaSettings.LinkPreviewRequest!.Topic, 1, 1)
                        .AddProducer(
                            kafkaSettings.MessageNotification.Producer,
                            producer => producer
                                .DefaultTopic(kafkaSettings.MessageNotification.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                        .AddProducer(
                            kafkaSettings.LinkPreviewRequest.Producer,
                            producer => producer
                                .DefaultTopic(kafkaSettings.LinkPreviewRequest.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                )
        );

        return services;
    }
}