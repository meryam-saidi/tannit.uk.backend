using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tannit.Application.Transactions.DTOs;

namespace Tannit.Application.Transactions.Commands.PurchaseTransaction
{
    public record PurchaseTransactionCommand(Guid UserId, decimal Amount, string PaymentMethod)
        : IRequest<TransactionDto>; // retourne l'Id de la transaction

}
