using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Domain.Entities
{
    public class PropertyImage : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCover { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;

        // Relation inverse
        public Property Property { get; set; } = default!;
    }
}
