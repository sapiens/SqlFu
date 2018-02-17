 
using FluentAssertions;
using Xunit;
using System;
using SqlFu.Builders;
using Xunit.Abstractions;


namespace Tests.Builders
{
    public class SqlStringBuilderTests
    {
        private readonly ITestOutputHelper _h;
        private SqlStringBuilder _sut;

        public SqlStringBuilderTests(ITestOutputHelper h)
        {
            _h = h;
            _sut=new SqlStringBuilder();
        }

       
    }
} 
