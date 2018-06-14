using System.Data.Common;

namespace SqlFu
{
    public static class DDLExtensions
    {
        

        public static void Truncate<T>(this DbConnection db)
        {
            var info = db.GetPocoInfo<T>();
            var name = info.EscapeName(db.Provider());
            db.Execute($"truncate {name}");
        }

        
        
      
    }
}