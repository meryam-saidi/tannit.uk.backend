using Tannit.Domain.Entities;

namespace Tannit.Application.Accounts
{
    public interface IUserAccountRepository
    {
        //Task<UserAccount?> GetByUserIdAsync(Guid userId);
        Task<UserAccount?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken ct = default);
        Task CreateIfNotExistsAsync(UserAccount account, CancellationToken ct = default);
        //Task IncrementBalanceAsync(Guid authUserId, decimal v, CancellationToken ct);
        //Task MarkSignupBonusGrantedAsync(Guid authUserId, CancellationToken ct);
        Task<bool> GrantSignupBonusAsync(Guid userId, decimal amount, CancellationToken ct = default);

    }
}
