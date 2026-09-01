using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Application.Abstractions.Notifications;

namespace TechChallenge.Infrastructure.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName));

        services.AddOptions<NotificationWorkerOptions>()
            .Bind(configuration.GetSection(NotificationWorkerOptions.SectionName));

        services.AddSingleton<IEmailSender, SmtpEmailSender>();

        if (configuration.GetValue<bool>($"{NotificationWorkerOptions.SectionName}:Enabled"))
            services.AddHostedService<NotificacaoStatusOutboxWorker>();

        return services;
    }
}
