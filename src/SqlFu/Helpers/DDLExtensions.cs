using System;
using System.Data.Common;
using SqlFu.Builders;
using SqlFu.Builders.CreateTable;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public static class DDLExtensions
    {
        public static void DropTable<T>(this DbConnection cnx)
        {
          var info = cnx.GetPocoInfo<T>();
           cnx.DropTable(info.Table.Name, info.Table.Schema);          
        }

        public static void DropTable(this DbConnection cnx, string name, string schema = "") 
            => cnx.GetProvider().DatabaseTools.DropTableIfExists(cnx,new TableName(name,schema));

        public static void Truncate<T>(this DbConnection db)
        {
            var info = db.GetPocoInfo<T>();
            var name = info.EscapeName(db.GetProvider());
            db.Execute($"truncate {name}");
        }

        public static bool TableExists(this DbConnection cnx, string name, string schema = null) 
            => cnx.GetProvider().DatabaseTools.TableExists(cnx,new TableName(name,schema));

        public static bool TableExists<T>(this DbConnection cnx)
        {
            var info = cnx.GetPocoInfo<T>();
            return cnx.GetProvider().DatabaseTools.TableExists(cnx,info.Table);
        }

        /// <summary>
        /// Generates and execute the table creation sql using the specified poco as the table representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cfg"></param>
        public static void CreateTableFrom<T>(this DbConnection db, Action<IConfigureTable<T>> cfg)
        {
            var provider = db.GetProvider();
            var tcfg = new TableConfigurator<T>(provider);
            cfg(tcfg);

            var data = tcfg.Data;

            var info = db.GetPocoInfo<T>();
           data.Update(info);

            var builder = new CreateTableBuilder(provider);
            
            if (db.TableExists<T>())
            {
                switch (tcfg.Data.CreationOptions)
                {
                    case IfTableExists.Throw:
                        throw new TableExistsException(tcfg.Data.TableName);
                    case IfTableExists.DropIt:
                        db.DropTable<T>();
                        break;
                      case IfTableExists.Ignore:
                        return;
                }          
            }


            db.Execute(c => c.Sql(builder.GetSql(tcfg.Data)));
        }

      
    }
}