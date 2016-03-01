using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SqlFu.Configuration;
using SqlFu.Providers;

namespace SqlFu.Builders.Expressions
{
    public class ExpressionSqlGenerator : ExpressionVisitor,IGenerateSqlFromExpressions
    {
        StringBuilder _sb;

        private readonly IDbProviderExpressions _provider;
        private readonly ITableInfoFactory _factory;
        private readonly IEscapeIdentifier _escape;


        public ExpressionSqlGenerator(IDbProviderExpressions provider, ITableInfoFactory factory, IEscapeIdentifier escape,ParametersManager param=null)
        {
            _sb = new StringBuilder();
            _provider = provider;
            _factory = factory;
            _escape = escape;
            Parameters = param ?? new ParametersManager();
        }

        public ParametersManager Parameters { get; }

        //string GetColumnName(LambdaExpression member)
        //{
        //    var mbody = member.Body as MemberExpression;
        //    if (mbody != null) return GetColumnName(mbody);
        //    var ubody = member.Body as UnaryExpression;
        //    if (ubody == null) throw new NotSupportedException("Only members and unary expressions are supported");
        //    return GetColumnName(ubody);
        //}

        private string GetColumnName(UnaryExpression member)
        {
            var mbody = member.Operand as MemberExpression;
            if (mbody != null) return GetColumnName(mbody);
            var ubody = member.Operand as UnaryExpression;
            if (ubody != null) return GetColumnName(ubody);
            throw new NotSupportedException("Only members and unary expressions are supported");
        }


        private string GetColumnName(MemberInfo column)
        {
            return _factory.GetInfo(column.DeclaringType).GetColumnName(column.Name, _escape);
        }


        private string GetColumnName(MemberExpression member)
        {
            var tableType = member.Expression.Type;
            var info = _factory.GetInfo(tableType);
            return info.GetColumnName(member, _escape);
        }

        public override string ToString() => _sb.ToString();


        bool IsSingleBooleanProperty(Expression node)
            => !_columnMode && (node.Type.Is<bool>() || node.Type.Is<bool?>()) && node.NodeType == ExpressionType.MemberAccess;

        bool IsSingleBooleanConstant(ConstantExpression node)
            => node != null && node.Type.Is<bool>();


        protected override Expression VisitBinary(BinaryExpression node)
        {
            string op = "";
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    op = "and";
                    break;
                case ExpressionType.OrElse:
                    op = "or";
                    break;
                case ExpressionType.Equal:
                    if (node.Right.IsNullUnaryOrConstant())
                    {
                        op = "is";
                        break;
                    }
                    if (IsSingleBooleanProperty(node.Left))
                    {
                        HandleSingleBooleanProperty(node.Left as MemberExpression, (bool)node.Right.GetValue());
                        return node;
                    }

                    op = "=";
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                case ExpressionType.NotEqual:

                    if (node.Right.IsNullUnaryOrConstant())
                    {
                        op = "is not";
                        break;
                    }
                    if (IsSingleBooleanProperty(node.Left))
                    {
                        HandleSingleBooleanProperty(node.Left as MemberExpression, false);
                        return node;
                    }
                    op = "<>";
                    break;
                case ExpressionType.Add:
                    op = "+";
                    break;
                case ExpressionType.Subtract:
                    op = "-";
                    break;
                case ExpressionType.Multiply:
                    op = "*";
                    break;
                case ExpressionType.Divide:
                    op = "/";
                    break;

                default:
                    throw new NotSupportedException();
            }
            _sb.Append("(");
            Visit(node.Left);

            _sb.Append($" {op} ");
            Visit(node.Right);

            _sb.Append(")");
            return node;
        }


        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                _sb.Append("null");
                return node;
            }



            WriteParameter(node);

            return node;
        }

        /// <summary>
        /// This is called only in criterias. It shouldn't be called when in generating columns
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                WriteParameter(node);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                _sb.Append("@" + Parameters.CurrentIndex);
                Parameters.AddValues(node.GetValue());
            }
            return node;
        }



        private void WriteParameter(Expression node)
        {
            _sb.Append("@" + Parameters.CurrentIndex);
            Parameters.AddValues(node.GetValue());
        }

        /// <summary>
        /// Names of the methods used to check if a column is contained by a collection
        /// </summary>
        static readonly string[] containsNames = new[] { "Contains", "HasValueIn" };

        private void HandleParameter(MethodCallExpression node)
        {

            if (node.HasParameterArgument())
            {
                if (containsNames.Contains(node.Method.Name))
                {
                    HandleContains(node);
                    return;
                }


                if (node.Method.Name == "InjectSql")
                {
                    HandleInject(node);
                    return;
                }

                _sb.Append(_provider.GetSql(node, new ExpressionSqlGenerator(_provider, _factory, _escape, Parameters)));
                return;
            }

            if (node.BelongsToParameter() && node.Object.Type.Is<string>())
            {
                HandleParamStringFunctions(node);
            }
        }

        private void HandleInject(MethodCallExpression node)
        {
            var sql = node.Arguments[1].GetValue() as string;
            var args = node.Arguments[2] as NewExpression;

            var i = 0;
            foreach (var arg in args.Members)
            {
                sql = sql.Replace($"@{arg.Name}", $"@{Parameters.CurrentIndex}");
                Parameters.AddValues(args.Arguments[i].GetValue());                
                i++;
            }
            _sb.Append(sql);
        }

        private void HandleParamStringFunctions(MethodCallExpression node)
        {
            var name = GetColumnName(node.Object as MemberExpression);

            object firstArg = null;
            if (node.Arguments.HasItems())
            {
                firstArg = node.Arguments[0].GetValue();
            }
            string value = "";
            switch (node.Method.Name)
            {
                case "StartsWith":
                    value = $"{name} like @{Parameters.CurrentIndex}";
                    Parameters.AddValues(firstArg + "%");
                    break;
                case "EndsWith":
                    value = $"{name} like @{Parameters.CurrentIndex}";
                    Parameters.AddValues("%" + firstArg);
                    break;
                case "Contains":
                    value = $"{name} like @{Parameters.CurrentIndex}";
                    Parameters.AddValues("%" + firstArg + "%");
                    break;
                case "ToUpper":
                case "ToUpperInvariant":
                    value = _provider.ToUpper(name);
                    break;
                case "ToLower":
                case "ToLowerInvariant":
                    value = _provider.ToLower(name);
                    break;
                case "Substring":
                    value = _provider.Substring(name, (int)firstArg, (int)node.Arguments[1].GetValue());
                    break;
            }

            _sb.Append(value);
        }

        private void HandleContains(MethodCallExpression meth)
        {
            IList list = null;
            var colIdx = 1;

            if (meth.Arguments.Count == 1)
            {
                list = meth.Object.GetValue() as IList;
                colIdx = 0;
            }
            else
            {
                var valIdx = meth.Arguments[0].BelongsToParameter() ? 1 : 0;
                colIdx = valIdx == 0 ? 1 : 0;
                list = meth.Arguments[valIdx].GetValue() as IList;
                if (list == null)
                {
                    var en = meth.Arguments[valIdx].GetValue() as IEnumerable;
                    var lst = new ArrayList();
                    foreach (var d in en) lst.Add(d);
                    list = lst;
                }


            }

            //if (list == null)
            //{
            //    throw new NotSupportedException("Contains must be invoked on a IList (array or List)");
            //}

            if (list.Count > 0)
            {
                var param = meth.Arguments[colIdx] as MemberExpression;
                _sb.Append(GetColumnName(param));

                _sb.AppendFormat(" in (@{0})", Parameters.CurrentIndex);
                Parameters.AddValues(list);
            }
            else
            {
                _sb.Append("1 = 0");
            }
        }


        private void HandleParameter(MemberExpression node)
        {
            if (!_columnMode && node.Type == typeof(bool))
            {
                HandleSingleBooleanProperty(node, true);
                return;
            }

            if (node.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(GetColumnName(node));
            }
            else
            {
                HandleParameterSubProperty(node);
            }



        }

        private void HandleSingleBooleanProperty(MemberExpression node, bool b)
        {
            _sb.Append(GetColumnName(node)).Append("=");
            VisitConstant(Expression.Constant(b));
        }


        /// <summary>
        /// For properties of a parameter property.
        /// Used to for properties that can be translated into db functions
        /// </summary>
        /// <param name="node"></param>
        private void HandleParameterSubProperty(MemberExpression node)
        {
            if (node.Expression.Type == typeof(string))
            {
                HandleStringProperties(node);
                return;
            }

            if (node.Expression.Type == typeof(DateTime) || node.Expression.Type == typeof(DateTimeOffset))
            {
                HandleDateTimeProperties(node);
                return;
            }
        }


        private void HandleStringProperties(MemberExpression node)
        {
            var name = (node.Expression as MemberExpression).Member.Name;
            switch (node.Member.Name)
            {
                case "Length":
                    _sb.Append(_provider.Length(name));
                    break;
            }
        }

        private void HandleDateTimeProperties(MemberExpression node)
        {
            var name = node.Expression.GetPropertyName();
            switch (node.Member.Name)
            {
                case "Year":
                    _sb.Append(_provider.Year(name));
                    break;
                case "Day":
                    _sb.Append(_provider.Day(name));
                    break;
            }
        }


        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            if (!_columnMode) return node;

            HandleObject(node.NewExpression, node.Bindings.Select(d => d.Member.Name).ToArray());
            
            foreach (var arg in node.Bindings.Cast<MemberAssignment>())
            {

                Visit(arg.Expression);
                _sb.AppendFormat(" as {0},", arg.Member.Name);

            }
            _sb.RemoveLastIfEquals(",");
            return node;
        }

        private void VisitProjection(NewExpression node)
        {
            if (node.Type.CheckIfAnonymousType())
            {
                HandleAnonymous(node);
                return;
            }

            HandleObject(node);
        }

        void HandleObject(NewExpression node, params string[] skip)
        {

            node.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(d => !skip.Any(s => d.Name == s))
                .ForEach(n =>
                {
                    _sb
                      //.AppendLine()
                      .Append(GetColumnName(n)).Append(",");

                });
            _sb.RemoveLastIfEquals(",");
        }

        private void HandleAnonymous(NewExpression node)
        {
            var i = 0;
            foreach (var arg in node.Arguments)
            {
                //_sb.AppendLine();
                Visit(arg);
                _sb.AppendFormat(" as {0},", node.Members[i].Name);
                i++;
            }
            _sb.RemoveLast();
        }

        protected override Expression VisitNew(NewExpression node)
        {
            VisitProjection(node);
            return node;
        }

        void HandleSingleBooleanConstant(ConstantExpression node)
        {
            var val = (bool)node.GetValue();
            _sb.Append(val ? "1=1" : "1<1");
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    if (IsSingleBooleanProperty(node.Operand))
                    {
                        HandleSingleBooleanProperty(node.Operand as MemberExpression, true);
                        break;
                    }
                    var op = node.Operand as ConstantExpression;
                    if (IsSingleBooleanConstant(op))
                    {
                        HandleSingleBooleanConstant(op);
                        return node;
                    }
                    Visit(node.Operand);
                    break;
                case ExpressionType.New:

                    Visit(node.Operand);
                    break;


                case ExpressionType.Not:
                    if (node.Operand.BelongsToParameter())
                    {
                        if (IsSingleBooleanProperty(node.Operand))
                        {
                            HandleSingleBooleanProperty(node.Operand as MemberExpression, false);
                            break;
                        }
                    }

                    var opf = node.Operand as ConstantExpression;
                    if (IsSingleBooleanConstant(opf))
                    {
                        HandleSingleBooleanConstant(opf);
                        return node;
                    }
                    _sb.Append("not ");

                    Visit(node.Operand);
                    break;
            }

            return node;
        }

        bool IsLambdaBooleanConstantHandled(LambdaExpression expression)
        {
            if (expression == null) return false;
            var body = expression.Body as ConstantExpression;
            if (IsSingleBooleanConstant(body))
            {
                HandleSingleBooleanConstant(body);
                return true;
            }
            return false;
        }

        /// <summary>
        /// For everything except "select columns"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string GetSql(Expression expression)
        {
            _sb.Clear();
            _columnMode = false;
            
           if (!IsLambdaBooleanConstantHandled(expression as LambdaExpression))
           {
                Visit(expression);
            }
            
            return _sb.ToString();
        }

        private bool _columnMode = false;

        /// <summary>
        /// Only to generate "select columns" 
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public string GetColumnsSql(params Expression[] columns)
        {
            _sb.Clear();
            _columnMode = true;
            columns.ForEach(col =>
            {
                Visit(col);
                _sb.Append(",");

            });
            _sb.RemoveLast();

            return _sb.ToString();

        }

    }


}