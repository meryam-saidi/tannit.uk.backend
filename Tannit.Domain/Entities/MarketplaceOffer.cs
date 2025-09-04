using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class MarketplaceOffer : BaseEntity
    {
        public Guid SellerId { get; set; }       // UserAccount.AuthUserId
        public Guid InvestmentId { get; set; }
        public decimal Price { get; set; }       // en TNP
        public bool IsActive { get; set; } = true;
    }
}
