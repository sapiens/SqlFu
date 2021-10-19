using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders;
using SqlFu.Builders.Crud;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    
    public static class CrudHelpers
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static InsertedId Insert<T>(this DbConnection db, T data, Action<IInsertableOptions<T>> cfg = null) where T:class
        {
            data.MustNotBeNull();
            if (data.IsAnonymousType()) cfg.MustNotBeNull("You need to specify table name at least");
            var info = db.GetPocoInfo<T>();
            var options = info.CreateInsertOptions<T>();
            
            cfg?.Invoke(options);

            var provider = db.Provider();
            var builder=new InsertSqlBuilder(data,provider,options);
            if (options.IdentityColumn.IsNullOrEmpty())
            {
                var count = db.Execute(builder.GetCommandConfiguration());
                if (count!=0) return InsertedId.OkWithNoResult;    
                return InsertedId.NotOkWithNoResult;
            }
            return db.GetValue<InsertedId>(builder.GetCommandConfiguration());
        }
        /// <summary>
        /// Inserts and ignores unique constraint exception.
        /// Useful when updating read models
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <param name="cfg"></param>
        /// <param name="keyName">unique constraint partial name</param>
     public static void InsertIgnore<T>(this DbConnection db, T data, Action<IInsertableOptions<T>> cfg = null,string keyName=null) where T : class
        {
            try
            {
                Insert(db, data, cfg);
            }
            catch (DbException ex) when (db.Provider().IsUniqueViolation(ex,keyName))
            {
                
                //ignore this    
            }
        }
     public static async Task InsertIgnoreAsync<T>(this DbConnection db, T data, CancellationToken? token = null, Action<IInsertableOptions<T>> cfg = null,string keyName=null) where T : class
     {
            try
            {
                await InsertAsync(db, data,token,cfg).ConfigureFalse();
            }
            catch (DbException ex) when (db.Provider().IsUniqueViolation(ex,keyName))
            {
                
                //ignore this    
            }
        }

        //todo refactor Insert in a class to deconstruct and reuse parts for more insert cases
        public static async Task<InsertedId> InsertAsync<T>(this DbConnection db, T data,CancellationToken? cancel=null ,Action<IInsertableOptions<T>> cfg = null) where T:class
        {
            data.MustNotBeNull();
            cancel = cancel ?? CancellationToken.None;
            if (data.IsAnonymousType()) cfg.MustNotBeNull("You need to specify table name at least");
            var info = db.GetPocoInfo<T>();
            var options = info.CreateInsertOptions<T>();
            cfg?.Invoke(options);

            var provider = db.Provider();
            var builder=new InsertSqlBuilder(data,provider,options);
            if (options.IdentityColumn.IsNullOrEmpty())
            {
                var count = await db.ExecuteAsync(builder.GetCommandConfiguration(),cancel.Value);
                if (count!=0) return InsertedId.OkWithNoResult;  
                return InsertedId.NotOkWithNoResult;
            }
            
            return await db.GetValueAsync<InsertedId>(builder.GetCommandConfiguration(), cancel.Value);
        }

        static Insertable<T> CreateInsertOptions<T>(this TableInfo info) =>new Insertable<T>(info);
            

        public static IBuildUpdateTable<T> Update<T>(this DbConnection db,Action<IHelperOptions> cfg=null) where T:class
        {
            var opt = new HelperOptions(db.GetPocoInfo<T>());
            cfg?.Invoke(opt); 
            var executor = new CustomSqlExecutor(db);
            return new UpdateTableBuilder<T>(executor, db.GetExpressionSqlGenerator(), db.Provider(), opt);
        }

        /// <summary>
        /// Perform update table with data from an anonymous object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="columns">Select which columns to update from an anonymous object</param>
        /// <param name="cfg">Configure name and other</param>
        /// <returns></returns>
        public static IBuildUpdateTableFrom<T> UpdateFrom<T>(this DbConnection db, Func<IUpdateColumns, IColumnsToUpdate<T>> columns, Action<IHelperOptions> cfg) where  T:class 
        {
            var options=new HelperOptions(db.GetPocoInfo<T>()); 
            var u = new UpdateColumns();
            cfg(options);
            
            var builder = columns(u) as UpdateColumns.CreateBuilder<T>;
            var executor=new CustomSqlExecutor(db);
            var updater=new UpdateTableBuilder<T>(executor,db.GetExpressionSqlGenerator(),db.Provider(),options);
            builder.PopulateBuilder(updater);
            return updater;
        }

        /// <summary>
        /// Perform update table with data from an anonymous object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="valuesToUpdate"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IBuildAnonymousUpdate UpdateFrom<T>(this DbConnection db, T valuesToUpdate, TableName tableName) where  T:class 
        {
            tableName.MustNotBeNull();
            var options=new HelperOptions(db.GetPocoInfo<T>());
            options.TableName = tableName;
            
            var executor=new CustomSqlExecutor(db);
            var updater=new UpdateTableBuilder<T>(executor,db.GetExpressionSqlGenerator(),db.Provider(),options);
            updater.SetUpdates(valuesToUpdate);

            return new UpdateAnonymousBuilder<T>(updater);
        }




        //public static int CountRows<T>(this DbConnection db,Expression<Func<T,bool>> condition=null)
        //    => db.QueryValue(d =>
        //    {
        //        var q=d.From<T>().Where(c=>true);
        //        if (condition != null)
        //        {
        //            q = q.And(condition);
        //        }
        //        return q.Select(c => c.Count());
        //    });
        

        public static int DeleteFromAnonymous<T>(this DbConnection db,T data,TableName tableName,Expression<Func<T, bool>> criteria = null)
        {
            var name = db.Provider().EscapeTableName(tableName);
            var builder = new DeleteTableBuilder(name, db.GetExpressionSqlGenerator());
            if (criteria != null) builder.WriteCriteria(criteria);
            return db.Execute(builder.GetCommandConfiguration());
        }

        public static int DeleteFrom<T>(this DbConnection db,Expression<Func<T, bool>> criteria=null)
        {
            var builder=new DeleteTableBuilder(db.GetTableName<T>(),db.GetExpressionSqlGenerator());
            if (criteria!=null) builder.WriteCriteria(criteria);
            return db.Execute(builder.GetCommandConfiguration());
        }

        public static Task<int> DeleteFromAsync<T>(this DbConnection db,CancellationToken? token=null,Expression<Func<T, bool>> criteria=null)
        {
            var builder=new DeleteTableBuilder(db.GetTableName<T>(), db.GetExpressionSqlGenerator());
            if (criteria!=null) builder.WriteCriteria(criteria);
            return db.ExecuteAsync(builder.GetCommandConfiguration(),token??CancellationToken.None);
        }
      
    }
}