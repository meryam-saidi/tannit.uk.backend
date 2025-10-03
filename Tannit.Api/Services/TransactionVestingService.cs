using Tannit.Application.Transactions;
using Tannit.Application.Transactions.DTOs;
using Tannit.Domain.Entities;

namespace Tannit.Api.Services
{
    public class TransactionVestingService : BackgroundService
    {
        //TransactionVestingService est enregistré comme Singleton dans Program.cs
        //Mais ITransactionRepository est enregistré comme Scoped
        //Problème : un singleton ne peut pas directement injecter un service scoped. Sinon il pourrait vivre plus longtemps que le scope et provoquer des erreurs.
        //Solution Possible: injection de IServiceScopeFactory dans le singleton (TransactionVestingService)
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransactionVestingService> _logger;        
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // TimeSpan.FromHours(1);

        public TransactionVestingService(
            IServiceScopeFactory scopeFactory,
            ILogger<TransactionVestingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TransactionVestingService démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                    var readyTransactions = await transactionRepo.GetSettledReadyToVestAsync(stoppingToken);

                    foreach (var tx in readyTransactions)
                    {
                        tx.Status = "Vested";
                        await transactionRepo.UpdateAsync(tx, stoppingToken);
                        _logger.LogInformation("Transaction {TransactionId} vestée.", tx.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du vesting des transactions.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
                //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            }
        }

        //Dès qu’un paiement est confirmé (par Stripe / PayPal / Konnect), on appelle SettleTransactionAsync
        //Ensuite, le service en background s’occupera de passer automatiquement à Vested après 15 jours.
        public async Task<TransactionDto?> SettleTransactionAsync(Guid transactionId)
        {
            using var scope = _scopeFactory.CreateScope();
            var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

            var tx = await transactionRepo.GetByIdAsync(transactionId, CancellationToken.None);
            if (tx == null) return null;

            tx.Status = "Settled";
            tx.SettledAtUtc = DateTime.UtcNow;

            await transactionRepo.UpdateAsync(tx, CancellationToken.None);

            return new TransactionDto(tx);
        }


    }

}
