using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tannit.Domain.Entities;

namespace Tannit.Application.Investments
{
    public interface IInvestmentRepository
    {
        Task CreateAsync(Investment investment);
        Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId);
    }
}
