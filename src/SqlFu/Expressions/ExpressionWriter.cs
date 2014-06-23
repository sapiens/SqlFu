using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlFu.Expressions
{
    public class ExpressionWriter : ExpressionVisitor
    {
        private readonly StringBuilder _sb;
        private readonly ParametersManager _pm;
        private readonly IDbProviderExpressionHelper _provider;

        public ExpressionWriter(StringBuilder sb, IDbProviderExpressionHelper provider, ParametersManager pm = null)
        {
            sb.MustNotBeNull();
            _sb = sb;
            if (pm == null)
            {
                pm = new ParametersManager();
            }
            _pm = pm;
            _provider = provider;
        }

        public ParametersManager Parameters
        {
            get { return _pm; }
        }


        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    break;
                case ExpressionType.Not:
                    if (node.Operand.BelongsToParameter())
                    {
                        if (node.Operand.NodeType == ExpressionType.MemberAccess)
                        {
                            var oper = node.Operand as MemberExpression;
                            if (oper.Type == typeof (bool))
                            {
                                var nex = EqualityFromUnary(node);
                                Visit(nex);
                                break;
                            }
                        }
                    }

                    //if (node.Operand is ConstantExpression)
                    //{
                    //    var oper = node.Operand as ConstantExpression;
                    //    if (oper.Type == typeof (bool))
                    //    {
                    //        _sb.Append(_provider.FormatBoolean(!(bool) oper.Value));
                    //        break;
                    //    }
                    //}

                    _sb.Append("not ");

                    Visit(node.Operand);
                    break;
            }

            return node;
        }

        private Expression EqualityFromUnary(UnaryExpression node)
        {
            return Expression.Equal(node.Operand,
                                    Expression.Constant(node.NodeType != ExpressionType.Not));
        }

        private Expression EqualityFromBoolProperty(Expression left, bool value)
        {
            return Expression.Equal(left, Expression.Constant(value));
        }


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
            if (ContinueAfterParameterBoolProperty(node.NodeType, node.Left))
            {
                Visit(node.Left);
            }
            _sb.Append(" " + op + " ");

            if (ContinueAfterParameterBoolProperty(node.NodeType, node.Right))
            {
                Visit(node.Right);
            }
            _sb.Append(")");
            return node;
        }

        private bool ContinueAfterParameterBoolProperty(ExpressionType type, Expression node)
        {
            if ((type == ExpressionType.AndAlso || type == ExpressionType.OrElse) && node.BelongsToParameter())
            {
                if (node is MemberExpression)
                {
                    var prop = node as MemberExpression;
                    if (prop.Type == typeof (bool))
                    {
                        var nex = Expression.Equal(prop, Expression.Constant(true));
                        Visit(nex);
                        return false;
                    }
                }
                else
                {
                    if (node is UnaryExpression)
                    {
                        var un = node as UnaryExpression;
                        if (un.Operand.Type == typeof (bool))
                        {
                            var nex = EqualityFromUnary(un);
                            Visit(nex);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                if (node.Type == typeof (string))
                {
                    _sb.AppendFormat("'{0}'", node.Value);
                }

                else
                {
                    if (node.Type == typeof (bool))
                    {
                        _sb.Append(_provider.FormatBoolean((bool) node.Value));
                    }
                    else
                    {
                        _sb.Append(node.Value);
                    }
                }
            }
            else
            {
                _sb.Append("null");
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.InvolvesParameter())
            {
                HandleParameter(node);
            }
            else
            {
                _sb.Append("@" + Parameters.CurrentIndex);
                Parameters.RegisterParameter(node.GetValue());
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
                Parameters.RegisterParameter(node.GetValue());
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            _sb.Append("@" + Parameters.CurrentIndex);
            Parameters.RegisterParameter(node.GetValue());
            return node;
        }

        private void HandleParameter(MethodCallExpression node)
        {
            if (node.HasParameterArgument())
            {
                if (node.Method.Name == "Contains")
                {
                    HandleContains(node);
                    return;
                }
                throw new NotSupportedException();
            }

            if (node.BelongsToParameter())
            {
                if (node.Object.Type == typeof (string))
                {
                    HandleParamStringFunctions(node);
                }
            }
        }

        private void HandleParamStringFunctions(MethodCallExpression node)
        {
            var name = node.Object.As<MemberExpression>().Member.Name;
            var arg = node.Arguments[0].GetValue();
            string value = null;
            switch (node.Method.Name)
            {
                case "StartsWith":
                    value = "{0} like @{1}".ToFormat(_provider.EscapeName(name), Parameters.CurrentIndex);
                    Parameters.RegisterParameter(arg + "%");
                    break;
                case "EndsWith":
                    value = "{0} like @{1}".ToFormat(_provider.EscapeName(name), Parameters.CurrentIndex);
                    Parameters.RegisterParameter("%" + arg);
                    break;
                case "Contains":
                    value = "{0} like @{1}".ToFormat(_provider.EscapeName(name), Parameters.CurrentIndex);
                    Parameters.RegisterParameter("%"+arg+"%");
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
                    value = _provider.Substring(name, (int) arg, (int) node.Arguments[1].GetValue());
                    break;
            }

            _sb.Append(value);
        }

        private void HandleContains(MethodCallExpression meth)
        {
            IList list = null;
            int pIdx = 1;
            if (meth.Arguments.Count == 1)
            {
                list = meth.Object.GetValue() as IList;
                pIdx = 0;
            }
            else
            {
                list = meth.Arguments[0].GetValue().As<IList>();
            }

            if (list == null)
            {
                throw new NotSupportedException("Contains must be invoked on a IList (array or List)");
            }
            var param = meth.Arguments[pIdx].As<MemberExpression>();

            _sb.Append(_provider.EscapeName(param.Member.Name)).AppendFormat(" in (@{0})", Parameters.CurrentIndex);
            Parameters.RegisterParameter(list);
        }

        private void HandleParameter(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.MemberAccess)
            {
                HandleParameter(node.Operand.As<MemberExpression>(), true);
            }
            else
            {
                Visit(node);
            }
        }

        private void HandleParameter(MemberExpression node, bool isSingle = false)
        {
            if (node.BelongsToParameter())
            {
                if (!isSingle)
                {
                    if (node.Expression.NodeType == ExpressionType.Parameter)
                    {
                        _sb.Append(_provider.EscapeName(node.Member.Name));
                    }
                    else
                    {
                        HandleParameterSubProperty(node);
                    }
                    return;
                }
                if (node.Type == typeof (bool))
                {
                    var eq = EqualityFromBoolProperty(node, true);
                    Visit(eq);
                    return;
                }
            }
        }

        /// <summary>
        /// For properties of a parameter property.
        /// Used to for properties that can be translated into db functions
        /// </summary>
        /// <param name="node"></param>
        private void HandleParameterSubProperty(MemberExpression node)
        {
            if (node.Expression.Type == typeof (string))
            {
                HandleStringProperties(node);
                return;
            }

            if (node.Expression.Type == typeof (DateTime) || node.Expression.Type == typeof (DateTimeOffset))
            {
                HandleDateTimeProperties(node);
                return;
            }
        }


        private void HandleStringProperties(MemberExpression node)
        {
            var name = node.Expression.As<MemberExpression>().Member.Name;
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

        public void Write<T>(Expression<Func<T, bool>> criteria)
        {
            criteria.MustNotBeNull();

            if (criteria.Body.BelongsToParameter())
            {
                switch (criteria.Body.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        HandleParameter(criteria.Body.As<MemberExpression>(), true);
                        return;
                    case ExpressionType.Not:
                        HandleParameter(criteria.Body.As<UnaryExpression>());
                        return;
                }
            }
            Visit(criteria);
        }

        public void Write<T>(Expression<Func<T, object>> criteria)
        {
            criteria.MustNotBeNull();
            if (criteria.Body.BelongsToParameter())
            {
                switch (criteria.Body.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        HandleParameter(criteria.Body.As<MemberExpression>(), true);
                        return;
                    case ExpressionType.Not:
                        HandleParameter(criteria.Body.As<UnaryExpression>());
                        return;
                }
            }
            Visit(criteria);
        }
    }

    public class ParametersManager
    {
        private readonly List<object> _params = new List<object>();

        public object[] ToArray()
        {
            return _params.ToArray();
        }


        /// <summary>
        /// param position
        /// </summary>
        public int CurrentIndex
        {
            get { return _params.Count; }
        }

        public void RegisterParameter(object value)
        {
            _params.Add(value);
        }
    }
}