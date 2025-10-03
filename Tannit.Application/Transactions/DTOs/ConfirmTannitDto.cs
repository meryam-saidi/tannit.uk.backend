using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Application.Transactions.DTOs
{
    public class ConfirmTannitDto
    {
        public Guid TransactionId { get; set; }          // ID de la transaction à confirmer
    }
}
