using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Logging;
using SqlFu.Executors;
using SqlFu.Executors.Resilience;

namespace SqlFu
{
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Sets the commands sql and parameters, takes care of provider specific functionality. At the end, the command is ready to execute
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, CommandConfiguration cfg)
        {
            var provider = cnx.Provider();
            var cmd = cnx.CreateCommand();
            cmd.CommandText = cfg.SqlText;
            
            if (cfg.IsStoredProcedure) cmd.CommandType=CommandType.StoredProcedure;
            var paramNames = cmd.SetupParameters(provider,cfg.Args);
            cmd.CommandText = provider.FormatParameters(cmd.CommandText, paramNames);
            provider.OnCommandExecuting(cmd);
            cfg.ApplyOptions(cmd);
            return cmd;
        }




        public static int Execute(this DbCommand cmd)
            => cmd.ExecuteNonQuery();
       

        /// <summary>
        /// Gets one result value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DbCommand cmd, Func<object, T> converter = null)
        {
            try
            {
                object rez = cmd.ExecuteScalar();
                SqlFuManager.Config.OnCommand(cmd);
                return SqlFuManager.GetConverter(converter)(rez);
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        /// <summary>
        /// Great of huge result sets, process one row at the time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="processor">Result processor</param>
        /// <param name="mapper"></param>
        /// <param name="firstRowOnly"></param>
        public static void QueryAndProcess<T>(this DbCommand cmd, Func<T, bool> processor, Func<DbDataReader, T> mapper = null,
                                     bool firstRowOnly = false)
        {
            var fuCommand = cmd.CastAs<SqlFuCommand>();
            var strat = fuCommand.GetErrorsStrategy();
            CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
            SqlFuCommand.HandleTransients(cmd, () =>
            {
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    while (reader.Read())
                    {
                        if (!processor(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader))) break;
                    }
                }

            },strat,fuCommand.Provider);
          
        }


        public static List<T> Fetch<T>(this DbCommand cmd, Func<DbDataReader, T> mapper = null,
                                    bool firstRowOnly = false)
        {
            var rez = firstRowOnly ? new List<T>(1) : new List<T>();

            cmd.QueryAndProcess(p =>
            {
                rez.Add(p);
                return true;
            },mapper,firstRowOnly);
            return rez;
           
        }

        #region Async

      

        public static async Task<int> ExecuteAsync(this DbCommand cmd, CancellationToken token)
        {
            int rez;
            try
            {
                rez = await cmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                SqlFuManager.Config.OnCommand(cmd);
                return rez;
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }


        public static async Task<T> GetValueAsync<T>(this DbCommand cmd, CancellationToken token, Func<object, T> converter = null)
        {
            try
            {
                object rez = await cmd.ExecuteScalarAsync(token).ConfigureAwait(false);
                SqlFuManager.Config.OnCommand(cmd);
                return SqlFuManager.GetConverter(converter)(rez);
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }
        /// <summary>
        /// Executes an async query then processes each result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="cancellation"></param>
        /// <param name="processor">Delegate to process a result item. Return false from delegate to end the query</param>
        /// <param name="mapper"></param>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        public static async Task QueryAndProcessAsync<T>(this DbCommand cmd, CancellationToken cancellation, Func<T, bool> processor, Func<DbDataReader, T> mapper = null,
                                      bool firstRowOnly = false)
        {
            try
            {
                CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
                using (var reader = await cmd.ExecuteReaderAsync(behavior, cancellation).ConfigureAwait(false))
                {
                    SqlFuManager.Config.OnCommand(cmd);

                    while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
                    {
                        if (!processor(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader))) break;
                    }
                }


            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task<List<T>> FetchAsync<T>(this DbCommand cmd, CancellationToken cancellation, Func<DbDataReader, T> mapper = null,
                                      bool firstRowOnly = false)
        {
            var rez = firstRowOnly ? new List<T>(1) : new List<T>();
            await cmd.QueryAndProcessAsync(cancellation, d =>
            {
                rez.Add(d);
                return true;
            },mapper,firstRowOnly);
            return rez;
            
        } 
        #endregion
    }
}