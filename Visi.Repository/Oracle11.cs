using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Query.Sql.Internal;
using Serilog;

namespace Visi.Repository;
#pragma warning disable EF1001
public static class Oracle11
{
    private class CustomOracleSqlGeneratorFactory(
        IOracleOptions oracleOptions,
        IRelationalTypeMappingSource typeMappingSource,
        CustomOracleQuerySqlGeneratorFactory customOracleQuerySqlGeneratorFactory)
    {
        public QuerySqlGenerator Create()
        {
            return new CustomOracleQuerySqlGenerator(customOracleQuerySqlGeneratorFactory._dependencies,
                typeMappingSource,
                oracleOptions.OracleSQLCompatibility);
        }
    }

    public class CustomOracleQuerySqlGeneratorFactory : OracleQuerySqlGeneratorFactory
    {
        private readonly CustomOracleSqlGeneratorFactory _customOracleSqlGeneratorFactory;
        public readonly QuerySqlGeneratorDependencies _dependencies;

        public CustomOracleQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource, IOracleOptions oracleOptions) : base(dependencies,
            typeMappingSource, oracleOptions)
        {
            _dependencies = dependencies;
            _customOracleSqlGeneratorFactory =
                new CustomOracleSqlGeneratorFactory(oracleOptions, typeMappingSource, this);
        }

        public override QuerySqlGenerator Create()
        {
            return _customOracleSqlGeneratorFactory.Create();
        }
    }

    private class CustomOracleQuerySqlGenerator(
        QuerySqlGeneratorDependencies dependencies,
        IRelationalTypeMappingSource typeMappingSource,
        OracleSQLCompatibility oracleSqlCompatibility)
        : OracleQuerySqlGenerator(dependencies, typeMappingSource, oracleSqlCompatibility)
    {
        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            if (selectExpression.Limit != null)
            {
                if (selectExpression.Offset == null)
                {
                    Sql.Append("SELECT * FROM (");
                }
                else
                {
                    Sql.Append("SELECT * FROM (");
                    Sql.Append("SELECT e.*, ROWNUM rnum FROM (");
                }
            }

            var expression = base.VisitSelect(selectExpression);

            if (selectExpression.Limit != null)
            {
                if (selectExpression.Offset == null)
                {
                    Sql.Append(") WHERE ROWNUM <= ");
                    Visit(selectExpression.Limit);
                }
                else
                {
                    Sql.Append(") e WHERE ROWNUM <= ");
                    Visit(selectExpression.Limit);
                    Sql.Append(" + ");
                    Visit(selectExpression.Offset);
                    Sql.Append(") WHERE rnum > ");
                    Visit(selectExpression.Offset);
                }
            }

            Log.Information("Generated SQL: {@sql}", Sql.ToString());

            return expression;
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
        }
    }
}
#pragma warning restore EF1001