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

        public async Task<IEnumerable<Property>> GetAllAsync1(CancellationToken ct)
        {
            const string sql = @"SELECT Id, Name, Description, TotalValue, TotalShares, AvailableShares, Address, Latitude, Longitude, LotNumber, LotArea, GESEmission, EnergyConsumption 
                                 FROM Properties";
            using var cn = _factory.Create();
            return await cn.QueryAsync<Property>(new CommandDefinition(sql, cancellationToken: ct));
        }
        public async Task<IEnumerable<Property>> GetAllAsync(CancellationToken ct)
        {
            const string sql = """
                                SELECT  p.*, 
                                        pi.Id, pi.PropertyId, pi.ImageUrl, pi.IsCover, pi.DisplayOrder
                                FROM Properties p
                                LEFT JOIN PropertyImages pi ON pi.PropertyId = p.Id
                                ORDER BY p.CreatedAtUtc DESC, pi.DisplayOrder
                                """;

            using var conn = _factory.Create();
            var map = new Dictionary<Guid, Property>();

            var result = await conn.QueryAsync<Property, PropertyImage, Property>(
                sql,
                (property, image) =>
                {
                    if (!map.TryGetValue(property.Id, out var p))
                    {
                        p = property;
                        p.Images = new List<PropertyImage>();
                        map.Add(p.Id, p);
                    }

                    if (image != null)
                        p.Images.Add(image);

                    return p;
                },
                splitOn: "Id"
            );

            return map.Values;
        }

        public async Task<Property?> GetByIdAsync1(Guid id, CancellationToken ct)
        {
            const string sql = @"SELECT Id, Name, Description, TotalValue, TotalShares, AvailableShares, Address, Latitude, Longitude, LotNumber, LotArea, GESEmission, EnergyConsumption
                                 FROM Properties 
                                 WHERE Id = @Id";
            using var cn = _factory.Create();
            return await cn.QuerySingleOrDefaultAsync<Property>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        }

        public async Task<Property?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
                                SELECT  p.*, 
                                        pi.Id, pi.PropertyId, pi.ImageUrl, pi.IsCover, pi.DisplayOrder
                                FROM Properties p
                                LEFT JOIN PropertyImages pi ON pi.PropertyId = p.Id
                                WHERE p.Id = @Id
                                ORDER BY pi.DisplayOrder
                                """;

            using var conn = _factory.Create();
            Property? property = null;

            await conn.QueryAsync<Property, PropertyImage, Property>(
                sql,
                (p, img) =>
                {
                    property ??= p;
                    property.Images ??= new List<PropertyImage>();
                    if (img != null)
                        property.Images.Add(img);
                    return property;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return property;
        }

        public async Task AddAsync(Property property, CancellationToken ct)
        {
            const string sql = @"INSERT INTO Properties 
                                (Id, Name, Description, TotalValue, TotalShares, AvailableShares, Address, Latitude, Longitude, LotNumber, LotArea, GESEmission, EnergyConsumption)
                                 VALUES (@Id, @Name, @Description, @TotalValue, @TotalShares, @AvailableShares, @Address, @Latitude, @Longitude, @LotNumber, @LotArea, @GESEmission, @EnergyConsumption)";
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
                                     AvailableShares = @AvailableShares, 
                                     Address = @Address, 
                                     Latitude = @Latitude, 
                                     Longitude = @Longitude,
                                     LotNumber = @LotNumber, 
                                     LotArea = @LotArea, 
                                     GESEmission = @GESEmission, 
                                     EnergyConsumption = @EnergyConsumption
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
