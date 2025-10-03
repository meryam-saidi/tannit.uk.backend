using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tannit.Application.Transactions;
using Tannit.Application.Transactions.DTOs;
using Tannit.Domain.Entities;

namespace Tannit.Application.Transactions.Commands.PurchaseTransaction
{
    public class PurchaseTransactionHandler : IRequestHandler<PurchaseTransactionCommand, TransactionDto>
    {
        private readonly ITransactionRepository _repo;

        public PurchaseTransactionHandler(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<TransactionDto> Handle(PurchaseTransactionCommand request, CancellationToken ct)
        {
            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = request.Amount,
                Currency = "Tannit",
                Type = "Purchase",
                Status = "Pending"
            };

            await _repo.CreateAsync(tx, ct);

            return new TransactionDto(tx);
        }
    }
}
