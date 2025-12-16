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

        // Méthodes métier
        Task DecreaseBalanceAsync(Guid userId, int amount);
        //Task IncreaseBalanceAsync(Guid userId, int amount);

        // Méthode générique (ADMIN / PROFIL)
        //Task UpdateAsync(User user);

        /*
         Règle d’or (à retenir pour le PFE)
         Les opérations qui ont une signification métier (DecreaseBalanceAsync&IncreaseBalanceAsync) doivent être explicites.
         Les opérations génériques (UpdateAsync) sont réservées aux cas non critiques.
        */
    }
}
