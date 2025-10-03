using MediatR;
using Tannit.Application.Accounts;
using Tannit.Application.Transactions;
using Tannit.Application.Transactions.Commands.ConfirmTransaction;
using Tannit.Domain.Entities;

namespace Tannit.Application.Accounts.Commands;

public sealed class EnsureUserAccountHandler : IRequestHandler<EnsureUserAccountCommand>
{
    private readonly IUserAccountRepository _repo;
    private readonly ITransactionRepository _txRepo; // pour journaliser
    private readonly IMediator _mediator;

    public EnsureUserAccountHandler(IUserAccountRepository repo, ITransactionRepository txRepo, IMediator mediator)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo)); // garde-fou
        _txRepo = txRepo;
        _mediator = mediator;
    }

    public async Task Handle(EnsureUserAccountCommand request, CancellationToken ct)
    {
        var acct = await _repo.GetByAuthUserIdAsync(request.AuthUserId, ct);
        if (acct is null)
        {
            // 1) Crée le compte à 0
            acct = new UserAccount
            {
                AuthUserId = request.AuthUserId,
                Email = request.Email,
                Balance = 0,
                VestedBalance = 0,
                KycStatus = "Unverified",
                CreatedAtUtc = DateTime.UtcNow,
                SignupBonusGranted = false
            };
            await _repo.CreateIfNotExistsAsync(acct, ct);
        }

        // Bonus d’inscription idempotent
        // Bonus atomique (40 Tannit)
        var granted = await _repo.GrantSignupBonusAsync(request.AuthUserId, 40m, ct);
        
        if (granted)
        {
            // 2) journaliser la transaction
            var transactionId = Guid.NewGuid();
            await _txRepo.CreateAsync(new Transaction
            {
                Id = transactionId,
                UserId = request.AuthUserId,
                Amount = 40m,
                Type = "SignupBonus",
                Currency = "Tannit",
                Status = "Settled",
                CreatedAtUtc = DateTime.UtcNow,
            }, ct);

            // 3) Settle la transaction
            await _mediator.Send(new ConfirmTransactionCommand(transactionId));
        }


    }
}

