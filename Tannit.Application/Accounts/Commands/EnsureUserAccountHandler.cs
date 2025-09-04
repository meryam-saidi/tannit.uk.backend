using MediatR;
using Tannit.Application.Accounts;
using Tannit.Application.Transactions;
using Tannit.Domain.Entities;

namespace Tannit.Application.Accounts.Commands;

public sealed class EnsureUserAccountHandler : IRequestHandler<EnsureUserAccountCommand>
{
    private readonly IUserAccountRepository _repo;
    private readonly ITransactionRepository _txRepo; // pour journaliser

    public EnsureUserAccountHandler(IUserAccountRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo)); // garde-fou
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

        // 2) Bonus d’inscription idempotent
        // Bonus atomique (40 Tannit)
        var granted = await _repo.GrantSignupBonusAsync(request.AuthUserId, 40m, ct);
        /*
        if (granted)
        {
            //journaliser la transaction
            await _txRepo.CreateAsync(new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = request.AuthUserId,
                Amount = 40m,
                Type = "SignupBonus",
                Currency = "Tannit",
                Status = "Settled",
                CreatedAtUtc = DateTime.UtcNow,
                SettledAtUtc = DateTime.UtcNow
            }, ct);
        }
        */
    }
}

