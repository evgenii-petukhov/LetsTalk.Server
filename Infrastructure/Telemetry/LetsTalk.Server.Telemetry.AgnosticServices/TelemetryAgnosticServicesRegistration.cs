using LetsTalk.Server.Telemetry.Azure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Telemetry.AgnosticServices;

public static class TelemetryAgnosticServicesRegistration
{
    public static IServiceCollection AddTelemetryAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("Features:Telemetry"))
        {
            case "Azure":
            default:
                services.AddAzureTelemetryServices();
                break;
        }

        return services;
    }

    public static WebApplication UseTelemetryFlush(this WebApplication app, IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("Features:Telemetry"))
        {
            case "Azure":
            default:
                return app.UseAzureTelemetryFlush();
        }
        
    }
}
