
namespace SqlFu.DDL
{
    public interface ICreateDDL
    {
        ICreateTable GetCreateTableBuilder(string name,IfTableExists option=IfTableExists.Throw/*,bool isTemporary=false*/);
        IModifyTable GetAlterTableBuidler(string name);        
    }
}