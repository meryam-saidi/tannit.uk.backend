using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tannit.Domain.Entities;

namespace Tannit.Application.Transactions
{
    public interface ITransactionRepository
    {
        Task CreateAsync(Transaction tx, CancellationToken ct = default);
        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
