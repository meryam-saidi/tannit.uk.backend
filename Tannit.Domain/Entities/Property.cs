using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class Property : BaseEntity
    {
        // Basic info
        public string Name { get; set; } = default!;
        public string Description { get; set; } = "";

        // Value & shares
        public decimal TotalValue { get; set; } // Valeur totale du bien
        public int TotalShares { get; set; }      // Nombre total de parts
        public decimal PricePerShare => TotalValue / TotalShares; // calculé
        public int AvailableShares { get; set; } // parts encore disponibles

        // Adress & location
        public string Address { get; set; } = "";        // Full address (for map integration)
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Details
        public List<PropertyImage> Images { get; set; } = new();
        public string LotNumber { get; set; } = "";      // Lot number or identifier Lot 12-A
        public decimal LotArea { get; set; }

        // Energy
        public decimal GESEmission { get; set; }         // greenhouse gas emission inKg CO2/m²/year
        public decimal EnergyConsumption { get; set; }  // kWh/m²/year
        public string EnergyClass
        {
            get
            {
                if (EnergyConsumption <= 50) return "A";
                if (EnergyConsumption <= 90) return "B";
                if (EnergyConsumption <= 150) return "C";
                if (EnergyConsumption <= 230) return "D";
                if (EnergyConsumption <= 330) return "E";
                if (EnergyConsumption <= 450) return "F";
                return "G";
            }
        }
    }
}
