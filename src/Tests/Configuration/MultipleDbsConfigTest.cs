 
using FluentAssertions;
using Xunit;
using System;
using SqlFu;
using Xunit.Abstractions;


namespace Tests.Configuration
{
    public class MultipleDbsConfigTest
    {
        private readonly ITestOutputHelper _h;
        private SqlFuConfig _cfg;

        public MultipleDbsConfigTest(ITestOutputHelper h)
        {
            _h = h;
            _cfg=new SqlFuConfig();
        }

        [Fact]
        public void automatic_class_generated_for_a_config_interface()
        {
            _cfg.AddProfile<IMainDb>(new FakeDbProvider(), "test");
            var d = _cfg.GetProfile(typeof(IMainDb).Name);
            d.Factory.Should().NotBeNull();
            
            d.Factory.Provider.Should().NotBeNull();
            d.Factory.Provider.Should().BeOfType<FakeDbProvider>();

        }

       
    }

    public interface IMainDb : IDbFactory
    {
        
    }
} 
