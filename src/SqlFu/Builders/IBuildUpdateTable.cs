using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders.Crud;

namespace SqlFu.Builders
{
    public interface IBuildUpdateTable<T>:IBuildUpdateTableFrom<T>
    {
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement);
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value);
        IBuildUpdateTable<T> Set(string propertyName, object value);
    }

    public interface IBuildUpdateTableFrom<T>:IExecuteSql
    {
     
        IExecuteSql Where(Expression<Func<T, bool>> criteria);        

    }


    public interface IBuildAnonymousUpdate : IExecuteSql
    {
        IExecuteSql Where<T>(T anonymousCriteria, Expression<Func<T, bool>> criteria = null) where T:class;
    }

    class UpdateAnonymousBuilder<R> : IBuildAnonymousUpdate
    {
        private readonly UpdateTableBuilder<R> _builder;

        public UpdateAnonymousBuilder(UpdateTableBuilder<R> builder)
        {
            _builder = builder;
        }

        public int Execute()=> _builder.Execute();

        public Task<int> ExecuteAsync(CancellationToken? token=null) => _builder.ExecuteAsync(token);
        

        public IExecuteSql Where<T>(T anonymousCriteria, Expression<Func<T, bool>> criteria = null) where T : class
        {
            anonymousCriteria.MustNotBeNull();
            if (criteria != null)
            {
                _builder.Where(criteria);
                return this;
            }
            foreach (var cv in anonymousCriteria.ValuesToDictionary())
            {
                _builder.WriteEqualityCriteria(cv.Key,cv.Value);
            }
            return this;
        }
    }

    public interface IUpdateColumns
    {
        /// <summary>
        /// Specifies the columns to be updated and which should be ignored
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Anonymous object</param>
       
        /// <returns></returns>
        IIgnoreColumns<T> Data<T>(T data) where T : class;

    }

    public interface IIgnoreColumns<T>:IColumnsToUpdate<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignore">List of columns to ignore</param>
        /// <returns></returns>
        IColumnsToUpdate<T> Ignore(params Expression<Func<T, object>>[] ignore);
    }
     public interface IIgnoreSelectColumns<T> where T:class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignore">List of columns to ignore</param>
        /// <returns></returns>
        void Ignore(params Expression<Func<T, object>>[] ignore);
    }

    public interface IExecuteSql
    {
        int Execute();
        Task<int> ExecuteAsync(CancellationToken? token=null);
    }
}