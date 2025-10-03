using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tannit.Domain.Entities;

namespace Tannit.Application.Transactions.DTOs
{
    public record TransactionDto(Guid Id, Guid UserId, decimal Amount, string Status, string Type, DateTime? VestsAtUtc)
    {
        public TransactionDto(Transaction tx) : this(tx.Id, tx.UserId, tx.Amount, tx.Status, tx.Type, tx.VestsAtUtc) { }
    }
}
