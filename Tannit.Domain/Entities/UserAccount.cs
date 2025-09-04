using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class UserAccount : BaseEntity
    {
        public Guid AuthUserId { get; set; }      // GUID venant du JWT (Auth Server)
        public string Email { get; set; } = default!;
        public decimal Balance { get; set; }      // Tannit Points (liquid)
        public decimal VestedBalance { get; set; } // Tannit Vested (après 15j)
        public string KycStatus { get; set; } = "Unverified";
        public int? CountryId { get; set; }
        public bool SignupBonusGranted { get; set; }
    }
}
