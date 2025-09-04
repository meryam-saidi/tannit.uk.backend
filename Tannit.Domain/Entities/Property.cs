using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class Property : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = "";
        public decimal TotalValue { get; set; } // Valeur totale du bien
        public int TotalShares { get; set; }      // Nombre total de parts
        public decimal PricePerShare => TotalValue / TotalShares; // calculé
        public int AvailableShares { get; set; } // parts encore disponibles
    }
}
