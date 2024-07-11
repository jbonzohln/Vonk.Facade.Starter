using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Oracle.EntityFrameworkCore.Query.Sql.Internal;

namespace Visi.Repository.Models;
#pragma warning disable EF1001
public class ViSiContext : DbContext
{
    private readonly IOptions<DbOptions> _dbOptionsAccessor;

    public ViSiContext(IOptions<DbOptions> dbOptionsAccessor)
    {
        _dbOptionsAccessor = dbOptionsAccessor;
    }

    public virtual DbSet<Child> Child { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        // optionsBuilder.UseSqlServer(_dbOptionsAccessor.Value.ConnectionString);
        optionsBuilder.UseOracle(_dbOptionsAccessor.Value.ConnectionString);
        optionsBuilder
            .ReplaceService<IQuerySqlGeneratorFactory, OracleQuerySqlGeneratorFactory,
                Oracle11.CustomOracleQuerySqlGeneratorFactory>();
    }
}
#pragma warning restore EF1001