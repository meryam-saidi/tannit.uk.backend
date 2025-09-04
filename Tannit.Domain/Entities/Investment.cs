using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class Investment : BaseEntity
    {
        public Guid UserId { get; set; }      // UserAccount.AuthUserId
        public Guid PropertyId { get; set; }
        public int TannitPoints { get; set; }
        public string Status { get; set; } = "Active"; // Active/SoldPartial/SoldFull
    }
}
