using System;
using System.Data.Common;
using CavemanTools.Model.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Tests.TestData;
using Xunit;

namespace Tests.Providers
{
    public class DbProviderTests
    {
        private FakeDbProvider _sut;

        public DbProviderTests()
        {
            _sut=new FakeDbProvider();
        }

        //[Fact]
        //public void value_objects_are_flattened_on_setup_param()
        //{
        //    var p = A.Fake<DbParameter>();
        //    _sut.SetupParameter(p,"t",new Email("t@example.com"));
        //    p.Value.Should().Be("t@example.com");
        //}

        //[Fact]
        //public void unregistered_value_objects_throws_on_setup()
        //{
        //    var p = A.Fake<DbParameter>();
        //    _sut.Invoking(d=>d.SetupParameter(p, "t", new Post()))
        //        .ShouldThrow<InvalidOperationException>()
        //        .Where(e=>e.Message.Contains(typeof(Post).ToString()));
            
        //}
    }
}