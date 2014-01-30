using System.Data.Common;
using SqlFu.Internals;

namespace SqlFu
{
    public static class ToolsExtensions
    {
        /// <summary>
        /// Drops table specified by type param
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        public static void Drop<T>(this DbConnection db,bool checkIfExists=true)
        {
            if (checkIfExists)
            {
                if (!db.TableExists<T>()) return;
            }
            var ti = TableInfo.ForType(typeof (T));
            db.DatabaseTools().DropTable(ti.Name);
        }

        /// <summary>
        /// Truncate table specified by type param
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        public static void Truncate<T>(this DbConnection db)
        {
            var ti = TableInfo.ForType(typeof (T));
            db.DatabaseTools().TruncateTable(ti.Name);
        }

        public static bool TableExists<T>(this DbConnection db)
        {
            var ti = TableInfo.ForType(typeof (T));
            return db.DatabaseTools().TableExists(ti.Name);
        }

        public static void CreateTable<T>(this DbConnection db)
        {
            db.DatabaseTools().GetCreateTableBuilder<T>().ExecuteDDL();
        }
    }
}