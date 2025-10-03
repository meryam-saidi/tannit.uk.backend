using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tannit.Application.Transactions.DTOs;

namespace Tannit.Application.Transactions.Commands.ConfirmTransaction
{
    public record ConfirmTransactionCommand(Guid TransactionId) : IRequest<TransactionDto>;

}
