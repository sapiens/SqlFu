﻿namespace SqlFu.Providers
{
    public interface IEscapeIdentifier
    {
        string EscapeIdentifier(string name);
        string EscapeTableName(TableName table);
    }

   
}