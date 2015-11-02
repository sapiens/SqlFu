using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SqlFu.Configuration;
using SqlFu.Providers;

namespace SqlFu.Builders.Expressions
{
    public class ExpressionWriter : ExpressionVisitor
    {
        private StringBuilder _sb;
        private readonly IDbProviderExpressions _provider;
        private readonly ExpressionWriterHelper _helper;


        public ExpressionWriter()
        {
            
        }
        public ExpressionWriter(IDbProviderExpressions provider,ExpressionWriterHelper helper,StringBuilder sb=null)
        {
            _sb = sb??new StringBuilder();
          
            _provider = provider;
            _helper = helper;
        }

        public ParametersManager Parameters { get; } = new ParametersManager();

        public ExpressionWriterHelper Helper
        {
            get { return _helper; }
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
            if (node.Value == null)
            {
                _sb.Append("null");
                return node;
            }

            WriteParameter(node);

            //if (node.Type == typeof (string))
            //{
            //    _sb.AppendFormat("'{0}'", node.Value);
            //}

            //else
            //{
            //    if (node.Type == typeof (bool))
            //    {
            //        _sb.Append(_provider.FormatConstant((bool) node.Value));
            //    }
            //    else
            //    {
            //        _sb.Append(node.Value);
            //    }
            //}
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
                Parameters.AddValues(node.GetValue());
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

        protected override Expression VisitNew(NewExpression node)
        {
            WriteParameter(node);
            return node;
        }

        private void WriteParameter(Expression node)
        {
            _sb.Append("@" + Parameters.CurrentIndex);
            Parameters.AddValues(node.GetValue());
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
                _provider.WriteMethodCall(node,_sb,_helper);                
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
            var name = GetColumnName(node.Object.As<MemberExpression>());
            
            object firstArg = null;
            if (node.Arguments.HasItems())
            {
                firstArg=node.Arguments[0].GetValue();
            }
            string value = null;
            switch (node.Method.Name)
            {
                case "StartsWith":
                    value = $"{name} like @{Parameters.CurrentIndex}";
                    Parameters.AddValues(firstArg + "%");
                    break;
                case "EndsWith":
                    value = "{0} like @{1}".ToFormat(name, Parameters.CurrentIndex);
                    Parameters.AddValues("%" + firstArg);
                    break;
                case "Contains":
                    value = "{0} like @{1}".ToFormat(name, Parameters.CurrentIndex);
                    Parameters.AddValues("%"+firstArg+"%");
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
                    value = _provider.Substring(name, (int) firstArg, (int) node.Arguments[1].GetValue());
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
            if (list.Count > 0)
            {
                var param = meth.Arguments[pIdx].As<MemberExpression>();
                _sb.Append(GetColumnName(param));
                _sb.AppendFormat(" in (@{0})", Parameters.CurrentIndex);
                Parameters.AddValues(list);
            }
            else
            {
                _sb.Append("1 = 0");
            }
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

        private string GetColumnName(MemberExpression node) => _helper.GetColumnName(node);

        private void HandleParameter(MemberExpression node, bool isSingle = false)
        {
            if (!node.BelongsToParameter()) return;
            if (!isSingle)
            {
                if (node.Expression.NodeType == ExpressionType.Parameter)
                {
                    _sb.Append(GetColumnName(node));                        
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
            }
        }

        //private void WriteColumnName(MemberExpression node)
        //{
        //    var col = _info.Columns.FirstOrDefault(c => c.PropertyInfo.Name == node.Member.Name);
        //    if (col==null) throw new InvalidOperationException("There is no column mapped to '{0}'".ToFormat(node.Member.Name));
        //    _sb.Append(_provider.EscapeIdentifier(col.Name));
        //}

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
           WriteCriteria(criteria);
        }

        public void WriteColumn(LambdaExpression property)
        {
            var convert = property.Body as UnaryExpression;
            if (convert != null)
            {
                var ct = convert.Operand as ConstantExpression;
                if (ct != null)
                {
                    _sb.Append(ct.Value);
                    return;
                }

                WriteColumn(convert.Operand as MemberExpression);
                return;
                
            }

           

            WriteColumn(property.Body as MemberExpression);
        }

        public void WriteColumn(MemberExpression property)
        {
            property.MustNotBeNull();
            if (!property.BelongsToParameter()) throw new InvalidOperationException("Property doesn't belong to a parameter");
            HandleParameter(property);           
        }

        

        public string GetSelectColumnsSql(params LambdaExpression[] columns)
        {
            var sb = _sb;
            _sb = new StringBuilder();
            columns.ForEach(col =>
            {
                switch (col.Body.NodeType)
                  {
                      case ExpressionType.MemberAccess:
                          Visit(col.Body as MemberExpression);
                          break;
                      case ExpressionType.New:
                        VisitProjection(col.Body as NewExpression);
                        break;
                    case ExpressionType.MemberInit:
                          VisitProjection(col.Body as MemberInitExpression);
                          break;
                    //case ExpressionType.Parameter:
                    //      _sb.AppendFormat("{0}.*", Manager.GetTableAlias(col.Body.As<ParameterExpression>().Type));                        
                    //      break;
                    default:
                          Visit(col.Body);
                          break;
                  }
                _sb.Append(",");

            });
            _sb.RemoveLast();
            var result = _sb.ToString();
            _sb = sb;
            return result;
        }

        private void VisitProjection(MemberInitExpression columns)
        {
            var node = columns;
                //columns.Body as NewExpression;
            var i = 0;
            foreach (var arg in node.Bindings.Cast<MemberAssignment>())
            {
                _sb.AppendLine();
                Visit(arg.Expression);
                _sb.AppendFormat(" as {0},", arg.Member.Name);
                i++;
            }
            _sb.RemoveLast();
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

        void HandleObject(NewExpression node)
        {
         
            node.Type.GetProperties(BindingFlags.Public|BindingFlags.Instance)
                .ForEach(n =>
                {
                    _sb.AppendLine().Append(_helper.GetColumnName(n)).Append(",");

                });
            _sb.RemoveLast();
        }

        private void HandleAnonymous(NewExpression node)
        {
            var i = 0;
            foreach (var arg in node.Arguments)
            {
                _sb.AppendLine();
                Visit(arg);
                _sb.AppendFormat(" as {0},", node.Members[i].Name);
                i++;
            }
            _sb.RemoveLast();
        }

        public string GetSql<T>(Expression<Func<T, object>> criteria)
        {
            var sb = _sb;
            _sb=new StringBuilder();
            Write(criteria);
            var result = _sb.ToString();
            _sb = sb;
            return result;
        }
        
        public string GetSql<T>(Expression<Func<T, bool>> criteria)
        {
            var sb = _sb;
            _sb=new StringBuilder();
            Write(criteria);
            var result = _sb.ToString();
            _sb = sb;
            return result;
        }

        public string GetCriteriaSql(LambdaExpression criteria)
        {
            var sb = _sb;
            _sb = new StringBuilder();
            WriteCriteria(criteria);
            var result = _sb.ToString();
            _sb = sb;
            return result;
        }
        
        public string GetExpressionSql(LambdaExpression expression)
        {
            var sb = _sb;
            _sb = new StringBuilder();
        
            WriteExpression(expression);
        
            var result = _sb.ToString();
            _sb = sb;
            return result;
        }

        public void WriteCriteria(LambdaExpression criteria)
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
            var constExpr = criteria.Body as ConstantExpression;
            if (constExpr != null)
            {
                if (constExpr.Type == typeof (bool))
                {
                    var val = (bool) constExpr.GetValue();
                    _sb.Append(val ? "1=1" : "1<1");
                    return;
                }
                
            }
            Visit(criteria);
        }

        public void WriteExpression(LambdaExpression expression)
        {
            expression.MustNotBeNull();
            if (expression.Body.BelongsToParameter())
            {
                switch (expression.Body.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        HandleParameter(expression.Body.As<MemberExpression>());
                        return;
                    case ExpressionType.Not:
                        HandleParameter(expression.Body.As<UnaryExpression>());
                        return;
                }
            }
            Visit(expression);
        }
        
        public void WriteExpression(Expression expression)
        {
            expression.MustNotBeNull();
            if (expression.BelongsToParameter())
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        HandleParameter(expression.As<MemberExpression>());
                        return;
                        case ExpressionType.Call:
                        HandleParameter(expression.As<MethodCallExpression>());
                        return;
                    case ExpressionType.Not:
                        HandleParameter(expression.As<UnaryExpression>());
                        return;
                }
            }
            Visit(expression);
        }

        public void Write<T>(Expression<Func<T, object>> expression)
        {
            WriteExpression(expression);
        }
    }

    
}