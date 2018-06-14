 
using FluentAssertions;
using Xunit;
using System;
using System.Data.Common;
using SqlFu;
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

        [Fact]
        public void add_formatted_string_as_raw()
        {
            var d = 0;
            _sut.AppendRaw($"bla {d}");
            _sut.GetCommandConfiguration().SqlText.Should().Be("bla 0");
        }

        [Fact]
        public void add_formatted_string_converts_arguments_to_db_params()
        {
            var d = 0;
            _sut.Append($"bla {d}");
            var cfg = _sut.GetCommandConfiguration();
            cfg.SqlText.Should().Be("bla @0");
            cfg.Args[0].Should().Be(0);
        }

        [Fact]
        public void conditional_false_append()
        {
            var d = 0;
            _sut.AppendIf(()=>false,$"bla {d}");
            _sut.GetCommandConfiguration().SqlText.Should().BeEmpty();
        }

        [Fact]
        public void conditional_true_append()
        {
            var d = 0;
            _sut.AppendIf(()=>true,$"bla {d}");
            _sut.GetCommandConfiguration().SqlText.Should().NotBeEmpty();
        }
    }
} 
