using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Query.Sql.Internal;
using Serilog;

namespace Visi.Repository;

public static class Oracle11
{
    public class CustomOracleSqlGeneratorFactory
    {
        private CustomOracleQuerySqlGeneratorFactory _customOracleQuerySqlGeneratorFactory;
        private readonly IOracleOptions _oracleOptions;
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public CustomOracleSqlGeneratorFactory(IOracleOptions oracleOptions,
            IRelationalTypeMappingSource typeMappingSource,
            CustomOracleQuerySqlGeneratorFactory customOracleQuerySqlGeneratorFactory)
        {
            _oracleOptions = oracleOptions;
            _typeMappingSource = typeMappingSource;
            _customOracleQuerySqlGeneratorFactory = customOracleQuerySqlGeneratorFactory;
        }

        public QuerySqlGenerator Create()
        {
            return new CustomOracleQuerySqlGenerator(_customOracleQuerySqlGeneratorFactory._dependencies,
                _typeMappingSource,
                _oracleOptions.OracleSQLCompatibility);
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

    private class CustomOracleQuerySqlGenerator : OracleQuerySqlGenerator
    {
        public CustomOracleQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource, OracleSQLCompatibility oracleSQLCompatibility) : base(
            dependencies, typeMappingSource, oracleSQLCompatibility)
        {
        }

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