using System;

namespace SqlFu.Migrations
{
    public class MigrationNotFoundException : Exception
    {
        public MigrationNotFoundException(string message) : base(message)
        {
        }
    }
}