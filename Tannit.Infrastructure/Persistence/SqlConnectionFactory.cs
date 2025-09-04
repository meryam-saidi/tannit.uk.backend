using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Tannit.Infrastructure.Persistence;
public interface IDbConnectionFactory { IDbConnection Create(); }

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _cs;
    public SqlConnectionFactory(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;
    public IDbConnection Create() => new SqlConnection(_cs);
}
