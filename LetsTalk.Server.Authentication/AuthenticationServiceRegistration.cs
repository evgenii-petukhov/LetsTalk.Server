using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Services;
using LetsTalk.Server.Configuration;

namespace LetsTalk.Server.Authentication
{
    public static class AuthenticationServiceRegistration
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
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
            services.AddGrpc();
            services.AddGrpcReflection();
            services.AddConfigurationServices(configuration);
            services.AddSingleton<IJwtService, JwtService>();
            return services;
        }
    }
}
