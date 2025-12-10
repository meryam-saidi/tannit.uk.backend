using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tannit.Domain.Entities;

namespace Tannit.Application.Properties
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync(CancellationToken ct);
        Task<Property?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Property property, CancellationToken ct);
        Task UpdateAsync(Property property, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
