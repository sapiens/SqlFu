using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using SqlFu;
using SqlFu.Providers.Sqlite;
using SqlFu.Providers.SqlServer;
using Tests.SqlServer;

namespace Tests.Usage
{
    public class SqliteTests:ADBOperationsTests
    {
        protected override DbConnection GetConnection()
        {
            return Setup.SqlFuConnection(new SqliteProvider(SQLiteFactory.Instance.CreateConnection), Setup.SqliteConnection);
        }

        protected override void Init()
        {
            _db.AddDbObjectOrIgnore($@"
create table {_db.GetTableName<User>()} (
Id integer primary key not null,
FirstName nvarchar(150),
LastName nvarchar(150),
Category varchar(10) default('Page'),
CreatedOn datetime DEFAULT CURRENT_TIMESTAMP,
IsDeleted bit default 0,
Posts int not null default 0,
unique(FirstName)
)

");
        }
    }
}