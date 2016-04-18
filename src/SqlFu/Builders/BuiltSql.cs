namespace SqlFu.Builders
{
    class BuiltSql<T> : IGenerateSql<T>
    {
        private CommandConfiguration _cmd;

        public BuiltSql(string sql, object[] args, HelperOptions options)
        {
            _cmd=new CommandConfiguration(sql,args);
            _cmd.ApplyOptions = options.CmdOptions;
        }

        public BuiltSql(CommandConfiguration cmd)
        {
            _cmd = cmd;
        }

        public IGenerateSql<TProjection> MapTo<TProjection>()
        {
            return new BuiltSql<TProjection>(_cmd);
        }

        public CommandConfiguration GetCommandConfiguration() => _cmd;
        
    }
}