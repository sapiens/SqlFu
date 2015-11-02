using System;
using System.Configuration;
using System.Data.Common;
using SqlFu.Builders.CreateTable;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public static class DDLExtensions
    {
        public static void DropTable<T>(this DbConnection cnx)
        {
          var info = SqlFuManager.Config.TableInfoFactory.GetInfo(typeof(T));
            cnx.DropTable(info.Name, info.DbSchema);
          
        }

        public static void DropTable(this DbConnection cnx, string name, string schema = "")
        {
            var provider = cnx.GetProvider();
            using (var cmd = cnx.CreateAndSetupCommand(provider.GetSqlForDropTableIfExists(name,schema)))
            {
                cmd.Execute();
            }
        }

        public static void Truncate<T>(this DbConnection db)
        {
            var info = db.GetPocoInfo<T>();
            var name = info.EscapeName(db.GetProvider());
            db.Execute($"truncate {name}");
        }

        public static bool TableExists(this DbConnection cnx, string name, string schema = null)
        {
            return cnx.GetProvider().DatabaseTools.TableExists(cnx, name,schema);
        }

        public static bool TableExists<T>(this DbConnection cnx)
        {
           var info = SqlFuManager.Config.TableInfoFactory.GetInfo(typeof(T));
            return cnx.TableExists(info.Name, info.DbSchema);
        }


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
                        throw new TableExistsException(tcfg.Data.Name);
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