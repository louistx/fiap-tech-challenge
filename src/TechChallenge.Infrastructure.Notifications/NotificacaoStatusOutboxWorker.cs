using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechChallenge.Application.Abstractions.Notifications;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Helpers;

namespace TechChallenge.Infrastructure.Notifications;

public class NotificacaoStatusOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NotificationWorkerOptions _options;
    private readonly ILogger<NotificacaoStatusOutboxWorker> _logger;

    public NotificacaoStatusOutboxWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<NotificationWorkerOptions> options,
        ILogger<NotificacaoStatusOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarLoteAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Falha inesperada ao processar notificações da outbox.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessarLoteAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificacaoStatusOutboxRepository>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var agora = DateTime.UtcNow;

        var notificacoes = await repository.ReservarPendentesAsync(
            agora,
            _options.BatchSize,
            TimeSpan.FromSeconds(_options.LockSeconds),
            cancellationToken);

        foreach (var notificacao in notificacoes)
            await ProcessarNotificacaoAsync(notificacao, repository, emailSender, cancellationToken);
    }

    private async Task ProcessarNotificacaoAsync(
        NotificacaoStatusOutbox notificacao,
        INotificacaoStatusOutboxRepository repository,
        IEmailSender emailSender,
        CancellationToken cancellationToken)
    {
        try
        {
            var statusAnterior = SystemHelper.GetStatusDescription(notificacao.StatusAnterior);
            var statusAtual = SystemHelper.GetStatusDescription(notificacao.StatusAtual);

            await emailSender.EnviarAsync(
                notificacao.Cliente.Email,
                $"OS {notificacao.CodigoAcompanhamento}: status atualizado",
                $"Olá, {notificacao.Cliente.Nome}.\n\n" +
                $"A ordem de serviço {notificacao.CodigoAcompanhamento} mudou de " +
                $"{statusAnterior} para {statusAtual}.\n\n" +
                "Consulte o acompanhamento da OS para mais detalhes.",
                cancellationToken);

            notificacao.MarcarComoEnviada(DateTime.UtcNow);
            await repository.SalvarAsync(cancellationToken);

            _logger.LogInformation(
                "Notificação {NotificacaoId} da OS {OrdemServicoId} enviada com sucesso.",
                notificacao.Id,
                notificacao.OrdemServicoId);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            notificacao.RegistrarFalha(exception.Message, DateTime.UtcNow);
            await repository.SalvarAsync(cancellationToken);

            _logger.LogWarning(
                exception,
                "Falha na tentativa {Tentativa} de enviar a notificação {NotificacaoId}.",
                notificacao.Tentativas,
                notificacao.Id);
        }
    }
}
