using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public class SqlQueryManager
    {
        private readonly IDbProvider _provider;
        private readonly ITableInfoFactory _factory;
        private readonly ParametersManager _pm;

        public SqlQueryManager(IDbProvider provider,ITableInfoFactory factory)
        {
            _pm=new ParametersManager();
            
            _provider = provider;
            _factory = factory;
            
        }

        public bool NoAliases { get; set; }
        public bool SimpleColumnNames { get; set; }

        public ParametersManager Parameters
        {
            get { return _pm; }
        }

        public IDbProvider Provider
        {
            get { return _provider; }
        }


        public ExpressionWriter CreateExpressionWriter(StringBuilder sb)
        {
            return new ExpressionWriter(Provider.GetExpressionsHelper(),this, sb);
        }

        Dictionary<Type,string> _tableNames=new Dictionary<Type, string>();

        private int idx = 0;

        /// <summary>
        /// Returns "[table_name] as [alias]". Only if NoAliases is false
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public string FormatTableName(Type tableType)
        {
            if (NoAliases)
            {
                return GetTableName(tableType);
            }
            return "{0} as {1}".ToFormat(GetTableName(tableType),GetTableAlias(tableType));
        }

        public string GetTableName(Type tableType)
        {
            return _factory.GetInfo(tableType).EscapeName(Provider);
        }

        public string GetTableAlias(Type tableType)
        {
            var name = _tableNames.GetValueOrDefault(tableType);
            if (name == null)
            {
                name = "t" + idx;
                _tableNames[tableType] = name;
                idx++;
            }
            return name;
        }

        public string GetColumnName(MemberExpression member,bool justTheName=false)
        {
            var tableType = member.Expression.Type;
            var col = _factory.GetInfo(tableType).GetColumnName(member, Provider);
            if (SimpleColumnNames|| justTheName) return col;
            return "{0}.{1}".ToFormat(NoAliases?GetTableName(tableType):GetTableAlias(tableType), col);
            
        }
        public string GetColumnName(string member,Type tableType)
        {
            var col = _factory.GetInfo(tableType).GetColumnName(member, Provider);
            return "{0}.{1}".ToFormat(NoAliases?GetTableName(tableType):GetTableAlias(tableType), col);
        }

    }
}