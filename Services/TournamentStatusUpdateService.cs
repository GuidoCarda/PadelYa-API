using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.Models.Tournament;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    /// <summary>
    /// Servicio que se ejecuta periódicamente para actualizar
    /// automáticamente los estados de los torneos según sus fechas.
    /// Este servicio revisa cada hora todos los torneos activos y:
    /// - Cambia torneos a "Finalizado" cuando pasa la fecha de fin del torneo
    /// </summary>
    public class TournamentStatusUpdateService : BackgroundService
    {
        private readonly ILogger<TournamentStatusUpdateService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Ejecutar cada 1 hora

        public TournamentStatusUpdateService(
            ILogger<TournamentStatusUpdateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TournamentStatusUpdateService iniciado. Se ejecutará cada {Interval} minutos.", 
                _checkInterval.TotalMinutes);

            // Ejecutar inmediatamente al iniciar
            await UpdateTournamentStatuses(stoppingToken);

            // Ejecutar periódicamente
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                    await UpdateTournamentStatuses(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Esto es normal cuando la aplicación se detiene
                    _logger.LogInformation("TournamentStatusUpdateService detenido.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en TournamentStatusUpdateService");
                }
            }
        }

        private async Task UpdateTournamentStatuses(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PadelYaDbContext>();

            try
            {
                var now = DateTime.UtcNow;
                var updatedCount = 0;

                _logger.LogInformation("Verificando estados de torneos a las {Time}", now);

                // Obtener todos los torneos que NO estén finalizados o eliminados
                var tournaments = await context.Tournaments
                    .Where(t => t.TournamentStatus != TournamentStatus.Finalizado &&
                               t.TournamentStatus != TournamentStatus.Deleted)
                    .ToListAsync(cancellationToken);

                foreach (var tournament in tournaments)
                {
                    var originalStatus = tournament.TournamentStatus;
                    var newStatus = DetermineNewStatus(tournament, now);

                    // Solo actualizar si el estado cambió
                    if (newStatus.HasValue && newStatus.Value != originalStatus)
                    {
                        tournament.TournamentStatus = newStatus.Value;
                        updatedCount++;

                        _logger.LogInformation(
                            "Torneo ID {TournamentId} '{Title}': Estado cambiado de {OldStatus} a {NewStatus}",
                            tournament.Id,
                            tournament.Title,
                            originalStatus,
                            newStatus.Value);
                    }
                }

                if (updatedCount > 0)
                {
                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Se actualizaron {Count} torneos automáticamente.", updatedCount);
                }
                else
                {
                    _logger.LogInformation("No se requieren actualizaciones de estado para ningún torneo.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estados de torneos");
            }
        }

        /// <summary>
        /// Determina el nuevo estado que debería tener un torneo según la fecha actual
        /// </summary>
        private TournamentStatus? DetermineNewStatus(Tournament tournament, DateTime now)
        {
            var currentStatus = tournament.TournamentStatus;

            // Regla: Si el torneo está en progreso y ya pasó la fecha de fin → cambiar a "Finalizado"
            if (now > tournament.TournamentEndDate && 
                currentStatus == TournamentStatus.EnProgreso)
            {
                return TournamentStatus.Finalizado;
            }

            // NOTA: No cambiamos automáticamente de "Abierto" a "En Progreso"
            // porque el admin debe generar el bracket primero.
            
            return null; // No hay cambio necesario
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TournamentStatusUpdateService se está deteniendo...");
            await base.StopAsync(cancellationToken);
        }
    }
}

