using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Application.Payments.DTOs
{
    public class CreatePaymentDto
    {
        //public Guid UserId { get; set; }
        //public int Tannit { get; set; }
        public int Amount { get; set; } // en £
        public string Currency { get; set; } = "gbp";

        //public string Method { get; set; } // stripe
    }
}
