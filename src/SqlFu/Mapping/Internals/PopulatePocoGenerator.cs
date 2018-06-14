using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using SqlFu.Configuration.Internals;

namespace SqlFu.Mapping.Internals
{
    class PopulatePocoGenerator<T>
    {
        private ParameterExpression _readerExpr;
        private MethodInfo _readValue;
        private ParameterExpression _customMapExpr;
        private ParameterExpression _arr;

        public PopulatePocoGenerator()
        {
            Converter = Expression.Parameter(typeof(IManageConverters),"converter");
            PocoExpr = Expression.Parameter(typeof(T), "poco");
            _customMapExpr = Expression.Parameter(typeof (IMapToPoco), "mapper");
            _readerExpr = Expression.Parameter(typeof (DbDataReader), "reader");
            _readValue = typeof(DbDataReader).GetMethod("GetValues", new[] { typeof(object[]) });
        }

        public ParameterExpression PocoExpr { get; }


        public ParameterExpression Converter { get; }

        public ParameterExpression ReaderExpr => _readerExpr;

        public ParameterExpression CustomMapExpr => _customMapExpr;

        public ParameterExpression ValuesVar => _arr;


        public Expression AssignCustomMappedValue(ColumnInfo data, string prefix,string queryId)
        {
            prefix = prefix + data.Name + "_";
            var callCustomMapper = Expression.Call(_customMapExpr,

                typeof (IMapToPoco).GetMethod("Map")

                .MakeGenericMethod(data.Type), _readerExpr,Expression.Constant(queryId),Expression.Constant(prefix));
            return Expression.Assign(GetPropertyExpression(data), callCustomMapper);
        }

        public IEnumerable<Expression> ReadValuesIntoArray(int fieldsCount)
        {
            _arr = Expression.Variable(typeof (object[]));
            var init = Expression.NewArrayBounds(typeof (object),Expression.Constant(fieldsCount));
            var arr=Expression.Assign(_arr, init);
           
            //reader.GetValues()
            var call=Expression.Call(_readerExpr, _readValue, _arr);
            return new Expression[] {arr,call};
        }


        public Expression AssignPropertyValue(ColumnInfo data, int i, bool justCast)
        {
       
            //arr[i]
            var valueExpr = Expression.ArrayIndex(_arr, Expression.Constant(i));

         

            //value=
            Expression value;
            if (justCast)
            {

                value=Expression.Condition(Expression.NotEqual(valueExpr, Expression.Constant(DBNull.Value)),
                    Expression.Convert(valueExpr, data.Type),GetPropertyExpression(data));
                
            }
            else{
                //converter.Convert<Type>(valueExpr)
                value=Expression.Call(Converter,
                    typeof (IManageConverters).GetMethod("Convert").MakeGenericMethod(data.Type),valueExpr);
            }
            
            //poco.Property=value
            return Expression.Assign(GetPropertyExpression(data),value);
        }

        private MemberExpression GetPropertyExpression(ColumnInfo data)
        {
            return Expression.Property(PocoExpr, data.PropertyInfo);
        }

       
    }
}