using SqlFu;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.Postgresql
{
    public class PostgresCreateTableTests:CommonCreateTableTests
    {
        private Stopwatch _t = new Stopwatch();

        public PostgresCreateTableTests()
        {

        }

        
        protected override DbEngine Engine
        {
            get { return DbEngine.PostgreSQL;}
        }

        public override void custom_constraints()
        {
            throw new NotImplementedException();
        }

       
    }
}