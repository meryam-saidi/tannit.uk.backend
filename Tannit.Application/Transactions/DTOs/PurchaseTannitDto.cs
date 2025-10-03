using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Application.Transactions.DTOs
{
    public class PurchaseTannitDto
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
    }

}
