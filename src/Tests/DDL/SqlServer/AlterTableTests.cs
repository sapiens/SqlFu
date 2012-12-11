using SqlFu;

namespace Tests.DDL.SqlServer
{
    public class AlterTableTests:CommonAlterTableTests
    {
        
       protected override DbEngine Engine
        {
            get { return DbEngine.SqlServer; }
        }


    }
}