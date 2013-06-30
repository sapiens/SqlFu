using System;
using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MysqlDropConstraintWriter : AbstractDropConstraintWriter
    {
        private const string PrimaryKey = "PRIMARY KEY";
        private readonly SqlFuConnection _db;

        public MysqlDropConstraintWriter(StringBuilder builder, SqlFuConnection db)
            : base(builder, DbEngine.MySql, db.DatabaseTools)
        {
            db.MustNotBeNull();
            _db = db;
        }

        protected override string EscapeName(string name)
        {
            return MySqlProvider.EscapeIdentifier(name);
        }

        protected override void WriteConstraint()
        {
            var type = GetConstraintType();
            //for now we don't care if the primary key is on an auto_increment column
            //if (type == PrimaryKey)
            //{
            //    var auto =
            //        _db.GetValue<bool>(
            //            @"select count(*) from information_schema.`COLUMNS` where TABLE_SCHEMA=@0 and TABLE_NAME=@1 and COLUMN_KEY='PRI' and EXTRA='auto_increment'");

            //}
            Builder.AppendFormat(type);
        }

        private string GetConstraintType()
        {
            var tp =
                _db.GetValue<string>(
                    @"select CONSTRAINT_TYPE as Type from information_schema.`TABLE_CONSTRAINTS` where CONSTRAINT_SCHEMA=@0 and CONSTRAINT_NAME=@1 and TABLE_NAME=@2 limit 1",
                    _db.Connection.Database, Item.Name ?? "PRIMARY", Item.TableName);
            if (tp.IsNullOrEmpty())
            {
                throw new InvalidOperationException("Constraint does not exist");
            }

            switch (tp)
            {
                case PrimaryKey:
                    return PrimaryKey;
                case "UNIQUE":
                    return Item.Name;
                case "FOREIGN KEY":
                    return tp + " " + Item.Name;
            }
            throw new NotSupportedException();
        }
    }
}