using Dapper;
using System.Data;
using Tannit.Application.Transactions;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Persistence;

namespace Tannit.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnectionFactory _factory;
        public TransactionRepository(IDbConnectionFactory factory) { _factory = factory; }

        public async Task CreateAsync(Transaction tx, CancellationToken ct)
        {
            using var cn = _factory.Create();
            const string sql = @"
            INSERT INTO Transactions (Id, UserId, Amount, Type, Currency, Status, CreatedAtUtc, SettledAtUtc)
            VALUES (@Id, @UserId, @Amount, @Type, @Currency, @Status, @CreatedAtUtc, @SettledAtUtc);";
            await cn.ExecuteAsync(sql, tx);
        }

        public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            const string sql = @"SELECT * FROM Transactions WHERE Id = @id";
            using var cn = _factory.Create();
            return await cn.QueryFirstOrDefaultAsync<Transaction>(
                new CommandDefinition(sql, new { id }, cancellationToken: ct)
            );
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            const string sql = @"SELECT * FROM Transactions WHERE UserId = @userId ORDER BY CreatedAtUtc DESC";
            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<Transaction>(
                new CommandDefinition(sql, new { userId }, cancellationToken: ct)
            );
            // Dapper retourne IEnumerable; si vous préférez matérialiser :
            return rows; // ou rows.ToList();
        }
    }
}
