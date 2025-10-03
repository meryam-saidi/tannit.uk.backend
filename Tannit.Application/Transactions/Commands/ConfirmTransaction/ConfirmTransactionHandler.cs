using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tannit.Application.Transactions;
using Tannit.Application.Transactions.DTOs;

namespace Tannit.Application.Transactions.Commands.ConfirmTransaction
{
    public class ConfirmTransactionHandler : IRequestHandler<ConfirmTransactionCommand, TransactionDto>
    {
        private readonly ITransactionRepository _repo;

        public ConfirmTransactionHandler(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<TransactionDto> Handle(ConfirmTransactionCommand request, CancellationToken ct)
        {
            var tx = await _repo.GetByIdAsync(request.TransactionId, ct);

            if (tx == null)
                throw new Exception("Transaction not found");

            tx.Status = "Settled";
            tx.SettledAtUtc = DateTime.UtcNow;
            tx.VestsAtUtc = DateTime.UtcNow.AddMinutes(3); //DateTime.UtcNow.AddDays(15)

            await _repo.UpdateAsync(tx, ct);

            return new TransactionDto(tx);
        }
    }
}
