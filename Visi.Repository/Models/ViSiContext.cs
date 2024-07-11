using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Oracle.EntityFrameworkCore.Query.Sql.Internal;

namespace Visi.Repository.Models;

#pragma warning disable EF1001
public class ViSiContext(IOptions<DbOptions> dbOptionsAccessor) : DbContext
{
    public virtual DbSet<Child> Child { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        // optionsBuilder.UseSqlServer(_dbOptionsAccessor.Value.ConnectionString);
        optionsBuilder.UseOracle(dbOptionsAccessor.Value.ConnectionString);
        optionsBuilder
            .ReplaceService<IQuerySqlGeneratorFactory, OracleQuerySqlGeneratorFactory,
                Oracle11.CustomOracleQuerySqlGeneratorFactory>();
    }
}
#pragma warning restore EF1001