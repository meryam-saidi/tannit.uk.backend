using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Application.Investments.DTO
{
    public class CreateInvestmentRequest
    {
        public Guid PropertyId { get; set; }
        public int TannitPoints { get; set; }
    }
}
