using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        //public Guid Id { get; set; }
        public Guid UserId { get; set; }     // UserAccount.AuthUserId
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "Tannit";
        public string Type { get; set; } = "";     // Top-Up - Purchase ...
        public string Status { get; set; } = "Pending";    // Pending - Settled - Vested - Sold(Cash)
        public DateTime? SettledAtUtc { get; set; }
        public DateTime? VestsAtUtc { get; set; }          // Created + 15j
    }
}
