using Dapper;
using System.Data;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Persistence;
using Tannit.Application.Accounts;

namespace Tannit.Infrastructure.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly IDbConnectionFactory _factory;
    public UserAccountRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<UserAccount?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken ct)
    {
        using var cn = _factory.Create();
        return await cn.QueryFirstOrDefaultAsync<UserAccount>(
            "SELECT * FROM UserAccounts WHERE AuthUserId=@id", new { id = authUserId });
    }
    
    /*
    public async Task<UserAccount?> GetByUserIdAsync(Guid authUserId)
    {
        using var cn = _factory.Create();
        return await cn.QueryFirstOrDefaultAsync<UserAccount>(
            "SELECT * FROM UserAccounts WHERE AuthUserId=@id", new { id = authUserId });
    }
    */

    public async Task CreateIfNotExistsAsync(UserAccount account, CancellationToken ct)
    {
        const string sql = @"
        IF NOT EXISTS (SELECT 1 FROM UserAccounts WHERE AuthUserId=@AuthUserId)
        INSERT INTO UserAccounts (AuthUserId, Email, Balance, VestedBalance, KycStatus, CreatedAtUtc, SignupBonusGranted)
        VALUES (@AuthUserId, @Email, 0, 0, 'Unverified', SYSUTCDATETIME(), @SignupBonusGranted);";
        using var cn = _factory.Create();
        await cn.ExecuteAsync(sql, account);
    }
    /*
    public async Task IncrementBalanceAsync(Guid userId, decimal amount, CancellationToken ct)
    {
        using var cn = _factory.Create();
        await cn.ExecuteAsync(
            "UPDATE UserAccounts SET Balance = Balance + @amount WHERE AuthUserId = @id",
            new { id = userId, amount });
    }

    public async Task MarkSignupBonusGrantedAsync(Guid userId, CancellationToken ct)
    {
        using var cn = _factory.Create();
        await cn.ExecuteAsync(
            "UPDATE UserAccounts SET SignupBonusGranted = 1 WHERE AuthUserId = @id",
            new { id = userId });
    }
    */

    public async Task<bool> GrantSignupBonusAsync(Guid userId, decimal amount, CancellationToken ct = default)
    {
        const string sql = @"
        UPDATE UserAccounts
        SET 
            Balance = Balance + @amount,
            SignupBonusGranted = 1
        WHERE AuthUserId = @id AND SignupBonusGranted = 0;";

        using var cn = _factory.Create();
        var rows = await cn.ExecuteAsync(sql, new { id = userId, amount });
        return rows == 1; // true si on vient d'accorder le bonus, false s'il était déjà accordé
    }
}
