using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Providers
{
    public abstract class AbstractProviderExpressions : IDbProviderExpressions
    {
        protected AbstractProviderExpressions()
        {
            Functions.Add(typeof(DbFunctions).GetMethod("Count").Name,Count);            
            Functions.Add(typeof(DbFunctions).GetMethod("Sum").Name,Sum);            
            Functions.Add(typeof(DbFunctions).GetMethod("Min").Name,Min);            
            Functions.Add(typeof(DbFunctions).GetMethod("Max").Name,Max);            
            Functions.Add(typeof(DbFunctions).GetMethod("Avg").Name,Avg);            
            Functions.Add(typeof(DbFunctions).GetMethod("Concat").Name,Concat);            
            Functions.Add(typeof(DbFunctions).GetMethod("Round").Name,Round);            
            Functions.Add(typeof(DbFunctions).GetMethod("Floor").Name,Floor);            
            Functions.Add(typeof(DbFunctions).GetMethod("Ceiling").Name,Ceiling);            
        }
        
        public string FormatConstant(string data)
        {
            return "'" + data + "'";
        }

        //public virtual string FormatConstant(bool data)
        //{
        //    return data ? "1" : "0";
        //}

        public virtual string Substring(string column, int pos, int length)
        {
            column.MustNotBeNull();
            var idx = pos + 1;
            return "substring({0},{1},{2})".ToFormat(column, idx, length);
        }

        public string ToUpper(string column)
        {
            return "upper({0})".ToFormat(column);
        }

        public string ToLower(string column)
        {
            return "lower({0})".ToFormat(column);
        }

        public virtual string Length(string column)
        {
            return "len({0})".ToFormat(column);
        }

        public virtual string Year(string column)
        {
            return "year({0})".ToFormat(column);
        }

        public virtual string Day(string column)
        {
            return "day({0})".ToFormat(column);
        }

        #region Db Functions
        protected Dictionary<string, Action<MethodCallExpression, StringBuilder,ExpressionWriterHelper>> Functions =new Dictionary<string, Action<MethodCallExpression, StringBuilder, ExpressionWriterHelper>>();

        private void Count(MethodCallExpression method, StringBuilder sb,ExpressionWriterHelper manager)
        {
            if (method.Arguments[0].NodeType==ExpressionType.Parameter)
            {
                 sb.Append(" count(*)");
                return;
            }

            var column = method.Arguments[0] as MemberExpression;
            var info = manager.GetColumnName(column);
            sb.Append(" count({0})".ToFormat(info));
        }

        private void Sum(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("sum(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")");
        }
        private void Min(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("min(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")");
        }
        private void Max(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("max(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")"); 
        }
        
        private void Avg(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("avg(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")"); 
        }
        
        private void Round(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("round(");
            var writer = manager.CreateExpressionWriter(sb);
            writer.WriteExpression(method.Arguments[0]);
            sb.Append(",");
            writer.WriteExpression(method.Arguments[1]);
            sb.Append(")"); 
        }
    
        private void Floor(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("floor(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")"); 
        }
    
        private void Ceiling(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("ceiling(");
            manager.CreateExpressionWriter(sb).WriteExpression(method.Arguments[0]);
            sb.Append(")"); 
        }
    


        private void Concat(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            sb.Append("concat(");
            var writer = manager.CreateExpressionWriter(sb);
            foreach (var arg in method.Arguments[0].As<NewArrayExpression>().Expressions)
            {
                writer.WriteExpression(arg);
                sb.Append(",");
            }
            sb.RemoveLastIfEquals(',');
            sb.Append(")"); 
        }

        public void WriteMethodCall(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            var func = Functions.GetValueOrDefault(method.Method.Name);
            if (func == null) throw new NotSupportedException("Unrecognized function {0}".ToFormat(method.Method.Name));
            func(method, sb,manager);
        } 
        #endregion
    }
}