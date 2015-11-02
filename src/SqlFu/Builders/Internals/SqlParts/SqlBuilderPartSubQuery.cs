using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartSubQuery:BaseSqlBuilderPart
    {
        private readonly ISqlBuilderPart _id;
        private readonly LambdaExpression _property;
        private readonly IBuildSql _subQuery;
        private readonly int _subCount;
        private readonly ExpressionWriter _wr;
        private readonly ParametersManager _pm;

        public SqlBuilderPartSubQuery(ISqlBuilderPart id,LambdaExpression property,IBuildSql subQuery,int subCount,ExpressionWriter wr,ParametersManager pm):base(id.PartId)
        {
            _id = id;
            _property = property;
            _subQuery = subQuery;
            _subCount = subCount;
            _wr = wr;
            _pm = pm;
        }

        string IntegrateSubquery()
        {
            var sql = _subQuery.GetSql();
            var replacer = new TableAliasesReplacer(_subCount);
            sql = replacer.Replace(sql);
            return Regex.Replace(sql, @"@[0-9]+", Updater, RegexOptions.Multiline);
          
        }

        private string Updater(Match match)
        {
            var paramIdx = int.Parse(match.ToString().Substring(1));
            return "@" + (paramIdx + _pm.CurrentIndex);
        }

      

        public override string ToString()
        {
            var queryParams = IntegrateSubquery();
            _pm.AddFrom(_subQuery);
            return string.Format("{0} {1} (\n{2}\n)",_property==null?"":_wr.GetExpressionSql(_property),_id,queryParams);
        }

        class TableAliasesReplacer
        {
            private readonly int _subCount;

            public TableAliasesReplacer(int subCount)
            {
                _subCount = subCount;
            }

            public IEnumerable<char> GetAliasPrefixes(string sql)
            {
                List<char> result = new List<char>();
                GetAliasChar(sql,result);
                return result;
            }

            private void GetAliasChar(string sql,List<char> items,int idx=0)
            {
                var fromIdx = sql.IndexOf("from",idx);
                if (fromIdx==-1) return;
                var asIdx = sql.IndexOf("as", fromIdx);
                if (asIdx == -1) return;
                items.Add(sql[asIdx + 3]);
                GetAliasChar(sql,items,asIdx+5);
            }

            public string Replace(string sql)
            {
                var chars = GetAliasPrefixes(sql).Reverse();
                foreach (var c in chars)
                {
                    sql = Regex.Replace(sql,@"[ \n\(]"+c + @"[0-9]+", Updater, RegexOptions.Multiline);
                }
                return sql;
            }

            private string Updater(Match match)
            {
                var value = match.ToString();
                var ch = value[1];
                return value.Replace(ch, (char)(ch - _subCount));
            }
        }
    }
}