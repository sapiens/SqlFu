using System;
using System.Data.Common;
using CavemanTools;

namespace SqlFu
{
    public class CommandConfiguration:IConfigureCommand
    {
        private Action<DbCommand> _applyOptions = Empty.ActionOf<DbCommand>();
        public string SqlText { get; set; }
        public object[] Args { get; set; }=new object[0];

        public bool IsStoredProcedure { get; set; }

        /// <summary>
        /// Applied just before executing the command
        /// </summary>
        public Action<DbCommand> ApplyOptions
        {
            get { return _applyOptions; }
            set
            {
                if (value==null) return;
                _applyOptions = value;
            }
        }

        public CommandConfiguration()
        {
            
        }

        public CommandConfiguration(string sql,params object[] args)
        {
            SqlText = sql;
            Args = args;
        }

        public IConfigureCommand Sql(string sqlText, params object[] args)
        {
            SqlText = sqlText;
            Args = args;
            return this;
        }

       
        public void Import(CommandConfiguration cfg)
        {
            Sql(cfg.SqlText, cfg.Args);
            ApplyOptions = cfg.ApplyOptions;
        }

        public IConfigureCommand WithCommandOptions(Action<DbCommand> options)
        {
            options.MustNotBeNull();
            ApplyOptions = options;
            return this;
        }
    }

    public interface IConfigureCommandOptions
    {
        /// <summary>
        /// Invoked just before the command execution. Used to configure the DbCommand object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IConfigureCommand WithCommandOptions(Action<DbCommand> options);
    }

    public interface IConfigureCommand : IConfigureCommandOptions
    {
        /// <summary>
        /// Assign sql and arguments
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IConfigureCommand Sql(string sqlText, params object[] args);

        void Import(CommandConfiguration cfg);
    }
}