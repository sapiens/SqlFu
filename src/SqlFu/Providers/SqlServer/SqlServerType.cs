using System;
using SqlFu.Providers.Sqlite;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerType:DbTypes
    {
        public const string Int = "int";
        public const string Float = "float";
        public const string TinyInt = "tinyint";
        public const string SmallInt = "smallint";
        public const string BigInt = "bigint";
        public const string Bit = "bit";
        public const string Numeric = "numeric";
        public const string Decimal = "decimal";
        public const string Money = "money";
        public const string SmallMoney = "smallmoney";
        public const string DateTimeOffset = "datetimeoffset";
        public const string DateTime = "datetime";
        public const string DateTime2 = "datetime2";
        public const string Date = "date";
        public const string Time = "time";
        public const string Char = "char";
        public const string NVarchar = "nvarchar";
        public const string Varchar = "varchar";
        public const string VarBinary = "varbinary";
        public const string Uuid = "uniqueidentifier";

        
        public SqlServerType()
        {
            this[typeof (byte)] = TinyInt;
            this[typeof (bool)] = Bit;
            this[typeof (bool?)] = Bit;
            this[typeof (byte?)] = TinyInt;
            this[typeof (double)] = Float;
            this[typeof (double?)] = Float;
            this[typeof (DateTime)] = DateTime;
            this[typeof (DateTime?)] = DateTime;
            this[typeof (DateTimeOffset)] = DateTimeOffset;
            this[typeof (DateTimeOffset?)] = DateTimeOffset;
            this[typeof (TimeSpan)] = Time;
            this[typeof (TimeSpan?)] = Time;
            this[typeof (string)] = NVarchar;
            this[typeof (byte[])] = VarBinary;
            this[typeof (Guid)] = Uuid;
            this[typeof (Guid?)] = Uuid;
            
            
        }
    }
}