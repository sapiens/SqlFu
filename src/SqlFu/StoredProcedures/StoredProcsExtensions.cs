using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Executors;

namespace SqlFu
{
    public static class StoredProcsExtensions
    {

        static bool IsOutput(string name) => name.StartsWith("_");


        public static SProcResult<T> QuerySProc<T>(this DbConnection db, Action<SProcInput> input)
        {
            return HandleProc(db, input, cmd =>
            {
                var r=new SProcResult<T>();
                r.Result = cmd.Fetch<T>();
                return r;
            }) as SProcResult<T>;
        }

        /// <summary>
        /// Executes sproc and maps the result. For just executing a sp, use 'ExecuteSProc'
        /// </summary>
        /// <param name="db"></param>
        /// <param name="procName"></param>
        /// <param name="cancel"></param>
        /// <param name="args">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// QuerySProc("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public static Task<SProcResult<T>> QuerySProcAsync<T>(this DbConnection db, string procName,CancellationToken cancel,object args=null)
        {
            return db.QuerySProcAsync<T>(s =>
            {
                s.ProcName = procName;
                s.Arguments = args;
            },cancel);
        }
        public static async Task<SProcResult<T>> QuerySProcAsync<T>(this DbConnection db, Action<SProcInput> input,CancellationToken cancel)
        {
            var res=await HandleProcAsync(db, input, async cmd =>
            {
                var r=new SProcResult<T>();
                r.Result = await cmd.FetchAsync<T>(cancel).ConfigureAwait(false);
                return r;
            });
            return res as SProcResult<T>;
        }      

        /// <summary>
        /// Executes sproc and maps the result. For just executing a sp, use 'ExecuteSProc'
        /// </summary>
        /// <param name="db"></param>
        /// <param name="procName"></param>
        /// <param name="args">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// QuerySProc("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public static SProcResult<T> QuerySProc<T>(this DbConnection db, string procName,object args=null)
        {
            return db.QuerySProc<T>(s =>
            {
                s.ProcName = procName;
                if (args != null) s.Arguments = args;
            });
        }

        private static SProcResult HandleProc(this DbConnection db, Action<SProcInput> input, Func<DbCommand,SProcResult> exec)
        {
            var data = new SProcInput();
            input(data);

            var output = new List<DbParameter>();
            var provider = db.Provider();

            DbParameter par = null;
            DbParameter returnValue = null;

            var cmdConfig = new CommandConfiguration();
            cmdConfig.IsStoredProcedure = true;
            cmdConfig.SqlText = data.ProcName;
            cmdConfig.ApplyOptions = data.Apply;
            SProcResult result ;
            using (var cmd = db.CreateAndSetupCommand(cmdConfig))
            {
                var args = PrepareStatement.CreateParamsDictionary(data.Arguments);
                args.ForEach(kv =>
                {
                    par = cmd.AddParam(provider, kv.Key, kv.Value);

                    if (IsOutput(kv.Key))
                    {
                        par.ParameterName = par.ParameterName[0]+ kv.Key.Substring(1);
                        par.Direction = ParameterDirection.Output;
                        output.Add(par);
                    }


                });

                returnValue = cmd.AddParam(provider, "ReturnValue", 0);
                returnValue.Direction=ParameterDirection.ReturnValue;
               
                result = exec(cmd);
            }
            
            result.ReturnValue = returnValue.Value.ConvertTo<int>();
            var outv = (IDictionary<string, object>) result.OutputValues;
            foreach (var param in output)
            {
                //first char of parameter name is db specific param prefix
                outv[param.ParameterName.Substring(1)] = param.Value;
            }
            return result;
        }

        private static async Task<SProcResult> HandleProcAsync(this DbConnection db, Action<SProcInput> input, Func<DbCommand,Task<SProcResult>> exec)
        {
            var data = new SProcInput();
            input(data);

            var output = new List<DbParameter>();
            var provider = db.Provider();

            DbParameter par = null;
            DbParameter returnValue = null;

            var cmdConfig = new CommandConfiguration();
            cmdConfig.IsStoredProcedure = true;
            cmdConfig.SqlText = data.ProcName;
            cmdConfig.ApplyOptions = data.Apply;
            SProcResult result ;
            using (var cmd = db.CreateAndSetupCommand(cmdConfig))
            {
                var args = PrepareStatement.CreateParamsDictionary(data.Arguments);
                args.ForEach(kv =>
                {
                    par = cmd.AddParam(provider, kv.Key, kv.Value);

                    if (IsOutput(kv.Key))
                    {
                        par.ParameterName = par.ParameterName[0]+ kv.Key.Substring(1);
                        par.Direction = ParameterDirection.Output;
                        output.Add(par);
                    }


                });

                returnValue = cmd.AddParam(provider, "ReturnValue", 0);
                returnValue.Direction=ParameterDirection.ReturnValue;
               
                result = await exec(cmd).ConfigureAwait(false);
            }
            
            result.ReturnValue = returnValue.Value.ConvertTo<int>();
            var outv = (IDictionary<string, object>) result.OutputValues;
            foreach (var param in output)
            {
                //first char of parameter name is db specific param prefix
                outv[param.ParameterName.Substring(1)] = param.Value;
            }
            return result;
        }


        /// <summary>
        /// Executes sproc. If you want to map a result from it, use 'QuerySProc'
        /// </summary>
        /// <param name="db"></param>
        /// <param name="procName"></param>
        /// <param name="args">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// ExecuteSProc("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public static SProcResult ExecuteSProc(this DbConnection db, string procName,object args=null)
        {
            return db.ExecuteSProc(s =>
            {
                s.ProcName = procName;
                if (args != null) s.Arguments = args;
            });
        }

        /// <summary>
        /// Executes sproc. If you want to map a result from it, use 'QuerySProc'
        /// </summary>
        /// <param name="db"></param>
        /// <param name="procName"></param>
        /// <param name="cancel"></param>
        /// <param name="args">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// ExecuteSProc("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public static Task<SProcResult> ExecuteSProcAsync(this DbConnection db, string procName,CancellationToken cancel,object args=null)
        {
            return db.ExecuteSProcAsync(s =>
            {
                s.ProcName = procName;
                s.Arguments = args;
            },cancel);
        }

        public static Task<SProcResult> ExecuteSProcAsync(this DbConnection db, Action<SProcInput> input,
            CancellationToken cancel)
        {
            return HandleProcAsync(db,input, async cmd =>
            {
                await cmd.ExecuteAsync(cancel).ConfigureAwait(false);
                return new SProcResult();
            });
        }

        public static SProcResult ExecuteSProc(this DbConnection db,Action<SProcInput> input)
        {
            return HandleProc(db, input, c =>
            {
                c.Execute();
                return new SProcResult();
            });        

        }

       
    }

   
}