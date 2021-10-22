using SqlFu.Configuration;
using SqlFu.Providers;
using System;

namespace Tests._Fakes
{
	public class FakeEscapeIdentifier : IEscapeIdentifier
	{

		public string EscapeIdentifier(string name)
		{
			name.MustNotBeEmpty();
			return name;
		}


		public string EscapeTableName(TableName table)
		{
			table.Name.MustNotBeEmpty();
			return table.Name;
		}
	}
}