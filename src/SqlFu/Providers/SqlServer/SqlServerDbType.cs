using System;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerDbType:DbTypes
    {
        public readonly string Int = "int";
        public readonly string Float = "float";
        public readonly string TinyInt = "tinyint";
        public readonly string SmallInt = "smallint";
        public readonly string BigInt = "bigint";
        public readonly string Bit = "bit";
        public readonly string Numeric = "numeric";
        public readonly string Decimal = "decimal";
        public readonly string Money = "money";
        public readonly string SmallMoney = "smallmoney";
        public readonly string DateTimeOffset = "datetimeoffset";
        public readonly string DateTime = "datetime";
        public readonly string DateTime2 = "datetime2";
        public readonly string Date = "date";
        public readonly string Time = "time";
        public readonly string NVarchar = "nvarchar";
        public readonly string VarBinary = "varbinary";
        public readonly string Uuid = "uniqueidentifier";

        
        public SqlServerDbType()
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