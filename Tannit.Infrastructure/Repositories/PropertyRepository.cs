using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Persistence;
using Tannit.Application.Properties;

namespace Tannit.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IDbConnectionFactory _factory;

        public PropertyRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<Property>> GetAllAsync(CancellationToken ct)
        {
            const string sql = @"SELECT Id, Name, Description, TotalValue, TotalShares, AvailableShares 
                                 FROM Properties";
            using var cn = _factory.Create();
            return await cn.QueryAsync<Property>(new CommandDefinition(sql, cancellationToken: ct));
        }

        public async Task<Property?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = @"SELECT Id, Name, Description, TotalValue, TotalShares, AvailableShares 
                                 FROM Properties 
                                 WHERE Id = @Id";
            using var cn = _factory.Create();
            return await cn.QuerySingleOrDefaultAsync<Property>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        }

        public async Task AddAsync(Property property, CancellationToken ct)
        {
            const string sql = @"INSERT INTO Properties 
                                (Id, Name, Description, TotalValue, TotalShares, AvailableShares)
                                 VALUES (@Id, @Name, @Description, @TotalValue, @TotalShares, @AvailableShares)";
            using var cn = _factory.Create();
            await cn.ExecuteAsync(new CommandDefinition(sql, property, cancellationToken: ct));
        }

        public async Task UpdateAsync(Property property, CancellationToken ct)
        {
            const string sql = @"UPDATE Properties 
                                 SET Name = @Name,
                                     Description = @Description,
                                     TotalValue = @TotalValue,
                                     TotalShares = @TotalShares,
                                     AvailableShares = @AvailableShares
                                 WHERE Id = @Id";
            using var cn = _factory.Create();
            await cn.ExecuteAsync(new CommandDefinition(sql, property, cancellationToken: ct));
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            const string sql = @"DELETE FROM Properties WHERE Id = @Id";
            using var cn = _factory.Create();
            await cn.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        }
    }
}
