﻿using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.DependencyInjection;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection DecorateScoped<TInterface, TDecorator>(
    this IServiceCollection services)
    where TInterface : class
    where TDecorator : TInterface
    {
        return services.AddScoped<TInterface>(serviceProvider =>
        {
            var service = services.FirstOrDefault(service => service.ServiceType == typeof(TInterface));
            return ActivatorUtilities.CreateInstance<TDecorator>(
                serviceProvider,
                ActivatorUtilities.CreateInstance(serviceProvider, service!.ImplementationType!));
        });
    }
}
