using System.Linq;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders
{
    public abstract class ASqlBuilder:IBuildSql
    {
        private readonly SqlQueryManager _manager;
        public const string Where_ = "where";
        public const string WhereEmpty = "";
        public const string WhereOr = "wor";
        public const string WhereAnd = "wand";
        public const string WhereIn = "win";
        public const string WhereNotIn = "wnotin";
        public const string Where_Exists = "wexists";
        public const string Where_NotExists = "wnot exists";
        public const string From = "from";
        public const string PreFrom = "prefrom";
        public const string PostFrom = "postfrom";
        public const string Select = "select";
        public const string JoinInner = "inner join";
        public const string JoinOuterLeft = "left join";
        public const string Group_By = "group by";
        public const string Having_ = "having";
        public const string HavingAnd = "hand";
        public const string HavingOr = "hor";
        public const string OrderBy_ = "orderby";
        public const string Limit_ = "lmit";

        protected ASqlBuilder(SqlQueryManager manager)
        {
            _manager = manager;
            _sb = new StringBuilder();
            _parts = new SqlParts(manager.CreateExpressionWriter(_sb));
            _expressionWriter=Manager.CreateExpressionWriter(Builder);
        }


        protected ExpressionWriter ExpressionWriter
        {
            get { return _expressionWriter; }
        }

        private StringBuilder _sb;

        private SqlParts _parts;
        private ExpressionWriter _expressionWriter;

        protected StringBuilder Builder
        {
            get { return _sb; }
        }

        protected ParametersManager Parameters
        {
            get { return Manager.Parameters; }
        }

        public SqlParts Parts
        {
            get { return _parts; }
        }

        protected SqlQueryManager Manager
        {
            get { return _manager; }
        }

        

        private string sql;

        public string GetSql()
        {
            if (sql == null)
            {
                BuildParts();
                sql= _sb.ToString();
            }
            return sql;
        }

        protected abstract void BuildParts();

        public object[] GetArguments()
        {
            return Parameters.ToArray();
        }

        protected ASqlBuilder WriteFrom<T>()
        {
            Builder.AppendFormat(" from {0}", Manager.FormatTableName(typeof(T)));
            return this;
        }

        protected void WriteCriteria()
        {
            WriteCriteria(new[] {Where_, WhereEmpty, WhereAnd, WhereOr, WhereIn, WhereNotIn, Where_Exists, Where_NotExists});
        }
        protected void WriteCriteria(string[] parts)
        {
            var criteria = Parts.GetParts(s =>parts.Any(d=>s==d));
            if (criteria.Length == 0) return;
            Builder.AppendLine();
            var useBlock = criteria.Length > 1;
            Builder.AppendFormat("{0} ",parts[0]);
            if (useBlock) Builder.Append("(");
            bool first=false;
            criteria.ForEach(c =>
            {
                if (c.PartId == parts[0])
                {
                    if (first)
                    {
                        Builder.Append(" and");
                    }
                    else
                    {
                        first = true;
                    }
                }
                
                Builder.AppendFormat(" {0}", c.ToString());
            });
            if (useBlock) Builder.Append(")");            
        }
    }


  
}