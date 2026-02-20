using LetsTalk.Server.API.Core.Services;
using LetsTalk.Server.Telemetry.Abstractions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LetsTalk.Server.Telemetry.Azure.Services;

public static class AzureTelemetryServicesRegistrations
{
    public static IServiceCollection AddAzureTelemetryServices(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnablePerformanceCounterCollectionModule = false;
            options.EnableRequestTrackingTelemetryModule = false;
            options.EnableDependencyTrackingTelemetryModule = false;
        });
        services.AddScoped<ITelemetryService, TelemetryService>();

        return services;
    }

    public static WebApplication UseAzureTelemetryFlush(this WebApplication app)
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            var telemetryConfiguration = app.Services.GetService<TelemetryConfiguration>();
            telemetryConfiguration?.Dispose();
        });

        return app;
    }
}
