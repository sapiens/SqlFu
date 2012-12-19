using System;
using System.Linq;
using CavemanTools.Infrastructure.MessagesBus;
using SqlFu.Migrations;

namespace Tests.Migrations
{
    public class FakeRunner : IRunMigrations
    {

        public void Run(params IMigrationTask[] tasks)
        {
            tasks.ForEach(t => t.Execute(null));
        }

        public IUnitOfWork StartUnitOfWork()
        {
            throw new NotImplementedException();
        }
    }
}