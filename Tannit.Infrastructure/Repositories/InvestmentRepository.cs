using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tannit.Application.Accounts;
using Tannit.Application.Investments;
using Tannit.Application.Properties;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Persistence;

namespace Tannit.Infrastructure.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly IDbConnectionFactory _factory;

        public InvestmentRepository(IDbConnectionFactory factory )
        {
            _factory = factory;
        }

        public async Task CreateAsync(Investment investment)
        {
            using var cn = _factory.Create();
            const string sql = @"
            INSERT INTO Investments 
            (Id, UserId, PropertyId, TannitPoints, Status)
            VALUES
            (@Id, @UserId, @PropertyId, @TannitPoints, @Status)
            ";

            await cn.ExecuteAsync(sql, investment);
        }

        public async Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId)
        {
            using var cn = _factory.Create();
            var sql = "SELECT * FROM Investments WHERE UserId = @UserId";
            return await cn.QueryAsync<Investment>(sql, new { UserId = userId });
        }

    }
}
