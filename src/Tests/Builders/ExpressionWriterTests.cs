using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Tests.Data;
using Xunit;
using SqlFu.Builders;
using SqlFu.Providers;
using System.Linq;
using SqlFu.Configuration;
using System.Collections.Generic;
using FakeItEasy;
using SqlFu;
using Tests._Fakes;

namespace Tests.Builders
{
    public class MyWriter : ExpressionVisitor
    {
        StringBuilder _sb;

        private readonly IDbProviderExpressions _provider;
        private readonly ITableInfoFactory _factory;
        private readonly IEscapeIdentifier _escape;


        public MyWriter(IDbProviderExpressions provider,ITableInfoFactory factory,IEscapeIdentifier escape)
        {
            _sb =  new StringBuilder();

            _provider = provider;
            _factory = factory;
            _escape = escape;
        }

        public ParametersManager Parameters { get; } = new ParametersManager();

        public string GetColumnName(LambdaExpression member)
        {
            var mbody = member.Body as MemberExpression;
            if (mbody != null) return GetColumnName(mbody);
            var ubody = member.Body as UnaryExpression;
            if (ubody == null) throw new NotSupportedException("Only members and unary expressions are supported");
            return GetColumnName(ubody);
        }

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


      

        private Expression EqualityFromUnary(UnaryExpression node)
            => Expression.Equal(node.Operand, Expression.Constant(node.NodeType != ExpressionType.Not));

        private Expression EqualityFromBoolProperty(Expression left, bool value)
            => Expression.Equal(left, Expression.Constant(value));

        public override string ToString() => _sb.ToString();


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
                    if (prop.Type == typeof(bool))
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
                        if (un.Operand.Type == typeof(bool))
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

            //_provider.WriteMethodCall(node, _sb, _helper);
            }

            if (node.BelongsToParameter())
            {
                if (node.Object.Type == typeof(string))
                {
                    HandleParamStringFunctions(node);
                }
            }
        }

        private void HandleParamStringFunctions(MethodCallExpression node)
        {
            var name = GetColumnName(node.Object as MemberExpression);

            object firstArg = null;
            if (node.Arguments.HasItems())
            {
                firstArg = node.Arguments[0].GetValue();
            }
            firstArg.MustNotBeNull();
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
            int pIdx = 1;
            if (meth.Arguments.Count == 1)
            {
                list = meth.Object.GetValue() as IList;
                pIdx = 0;
            }
            else
            {
                list = meth.Arguments[0].GetValue() as IList;
            }

            if (list == null)
            {
                throw new NotSupportedException("Contains must be invoked on a IList (array or List)");
            }

            if (list.Count > 0)
            {
                var param = meth.Arguments[pIdx] as MemberExpression;
                _sb.Append(GetColumnName(param));

                _sb.AppendFormat(" in (@{0})", Parameters.CurrentIndex);
                Parameters.AddValues(list);
            }
            else
            {
                _sb.Append("1 = 0");
            }
        }

        //private void HandleParameter(UnaryExpression node)
        //{
        //    if (node.NodeType == ExpressionType.MemberAccess)
        //    {
        //        HandleParameter(node.Operand as MemberExpression, true);
        //    }
        //    else
        //    {
        //        Visit(node);
        //    }
        //}

        
        private void HandleParameter(MemberExpression node)
        {
            if (_columnMode)
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
            if (node.Type == typeof(bool))
            {
                HandleSingleBooleanProperty(node,true);

                //var eq = EqualityFromBoolProperty(node, true);
                //Visit(eq);
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


       
        private void VisitProjection(MemberInitExpression columns)
        {
            var node = columns;

            foreach (var arg in node.Bindings.Cast<MemberAssignment>())
            {
                _sb.AppendLine();
                Visit(arg.Expression);
                _sb.AppendFormat(" as {0},", arg.Member.Name);

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

            node.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ForEach(n =>
                {
                  _sb
                    //.AppendLine()
                    .Append(GetColumnName(n)).Append(",");

                });
            _sb.RemoveLast();
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
            //WriteParameter(node);
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
                    var op = node.Operand as ConstantExpression;
                    if (op != null && op.Type == (typeof (bool)))
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
                        if (node.Operand.NodeType == ExpressionType.MemberAccess)
                        {
                            var oper = node.Operand as MemberExpression;
                            if (oper.Type == typeof(bool))
                            {
                               HandleSingleBooleanProperty(oper,false);
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

        public string GetSql(LambdaExpression expression)
        {
            _sb.Clear();
            _columnMode = false;
            Visit(expression);
            return _sb.ToString();
        }

        private bool _columnMode = false;

        public string GetColumnsSql(params LambdaExpression[] columns)
        {
            _sb.Clear();
            _columnMode = true;
            columns.ForEach(col =>
            {
                Visit(col.Body);
                //switch (col.Body.NodeType)
                //{
                //    case ExpressionType.MemberAccess:
                //        VisitColumn(col.Body as MemberExpression);
                //        break;
                //    case ExpressionType.New:
                //        VisitProjection(col.Body as NewExpression);
                //        break;
                //    case ExpressionType.MemberInit:
                //        VisitProjection(col.Body as MemberInitExpression);
                //        break;

                //    default:
                //        Visit(col.Body);
                //        break;
                //}
                _sb.Append(",");

            });
            _sb.RemoveLast();
         
           return _sb.ToString();
           
        }

    }

    public class ExpressionWriterTests
    {
        private MyWriter _sut;
        Expression<Func<MapperPost, object>> _l;

        public ExpressionWriterTests()
        {
            _sut = new MyWriter(A.Fake<IDbProviderExpressions>(), Setup.InfoFactory(), FakeEscapeIdentifier.Instance);
        }

        [Fact]
        public void single_constant_true()
        {
           _l = d => true;
            var sql = _sut.GetSql(_l);
            sql.Should().Be("1=1");
        }

        [Fact]
        public void single_constant_false()
        {
           _l = d => false;
            var sql = _sut.GetSql(_l);
            sql.Should().Be("1<1");
        }

        string Get(Expression<Func<MapperPost, object>> d) => _sut.GetSql(d);

        [Fact]
        public void single_constant_value()
        {
            var sql = Get(d => 2);
            sql.Should().Be("@0");
            _sut.Parameters.ToArray().Should().BeEquivalentTo(new[] {2});
        }

        [Fact]
        public void criteria_is_boolean_column()
        {
            var sql = Get(d => d.IsActive);
            sql.Should().Be("IsActive=@0");
            _sut.Parameters.ToArray().First().Should().Be(true);
        }

        class IdName
        {
            public int Id { get; set; }
            public string Name { get; set; } 
        }

        [Fact]
        public void get_projection_from_new_object()
        {
            var sql = Get(d => new IdName());
            sql.Should().Be("Id,Name");
        }

        [Fact]
        public void get_projection_from_anonymous()
        {
            _l = d => new {d.Id, Name = d.Title};
            var sql = _sut.GetColumnsSql(_l);
            sql.Should().Be("Id as Id,Title as Name");
        }

        [Fact]
        public void single_boolean_property_is_negated()
        {
            var sql = Get(d => !d.IsActive);
            sql.Should().Be("IsActive=@0");
            _sut.Parameters.ToArray().First().Should().Be(false);
        }
        [Fact]
        public void property_as_column_name()
        {
            _l = d => d.IsActive;
            var sql = _sut.GetColumnsSql(_l);
            sql.Should().Be("IsActive");
            
        }
    }
}