using System.Data;
using Moq;
using SqlFu;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests
{
    public class AnonymousMapping
    {
        private Stopwatch _t = new Stopwatch();
        private Mock<IDataReader> _rd;
        private Type _tp;

        public AnonymousMapping()
        {
            _rd = new Mock<IDataReader>();
            var dt = new { Id = 23, Name = "" };
            _tp = dt.GetType();
            _rd.Setup(d => d["Id"]).Returns(11);
            _rd.Setup(d => d["Name"]).Returns("bla");
            _rd.Setup(d => d["Extra"]).Returns("bla");
            _rd.Setup(d => d.FieldCount).Returns(2);
        }

        [Fact]
        public void map_anonymous_object_with_less_matching_names()
        {
            _rd.Setup(d => d.FieldCount).Returns(1);
            Assert.Throws<InvalidOperationException>(() => PocoFactory.AnonMapper(_tp)(_rd.Object));
            
        }

        [Fact]
        public void simple_mapping_from_query()
        {
            dynamic d = PocoFactory.AnonMapper(_tp)(_rd.Object);
            Assert.Equal(11,d.Id);
            Assert.Equal("bla",d.Name);
        }

        
        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}