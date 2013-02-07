using System.Data;
using System.Globalization;
using Moq;
using SqlFu;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Mapper
{
    public class PocoB
    {
        public PocoC Item { get; set; }
    }

    public class PocoC
    {
        public Guid Id { get; set; }
        public Guid? Id1 { get; set; }
        public int? Id2 { get; set; }
        public TimeSpan? Time { get; set; }
        public IsolationLevel Enum { get; set; }
        public CultureInfo Info { get; set; }
    }

    public class PocoA
    {
        public PocoB Item { get; set; }
    }
    public class ComplexMapperTests
    {
        private Stopwatch _t = new Stopwatch();
        private DefaultComplexTypeMapper _mapper;
        private Mock<IDataReader> _reader;
        private PocoA _poco;

        public ComplexMapperTests()
        {
            _mapper = new DefaultComplexTypeMapper();
            _reader = new Mock<IDataReader>();
            _poco = new PocoA();
        }

        [Fact]
        public void guid_type_is_handled()
        {
            var poco = new PocoA();
            var id = Guid.NewGuid();
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Id");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            _reader.Setup(d => d[0]).Returns(id);
            _mapper.MapType(poco,_reader.Object,0);
            Assert.Equal(id,poco.Item.Item.Id);
        }

        [Fact]
        public void different_separator()
        {
            var poco = new PocoA();
            _mapper.Separator = '.';
            var id = Guid.NewGuid();
            _reader.Setup(d => d.GetName(0)).Returns("Item.Item.Id");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            _reader.Setup(d => d[0]).Returns(id);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(id, poco.Item.Item.Id);
        }

        [Fact]
        public void nullable_guids_are_handled()
        {
            var poco = new PocoA();
            var id = Guid.NewGuid();
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Id1");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            
            _reader.Setup(d => d[0]).Returns(id);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(id,poco.Item.Item.Id1);

           
            _reader.Setup(d => d.IsDBNull(0)).Returns(true);
            _reader.Setup(d => d[0]).Returns(null);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Null(poco.Item.Item.Id1);
        }

        [Fact]
        public void nullable_types_are_handled()
        {
            var poco = new PocoA();
            var id = 2;
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Id2");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
          _reader.Setup(d => d[0]).Returns(id);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(id, poco.Item.Item.Id2.Value);

            _reader.Setup(d => d.IsDBNull(0)).Returns(true);
            _reader.Setup(d => d[0]).Returns(null);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Null(poco.Item.Item.Id2);
        }

        [Fact]
        public void timespan_is_handled()
        {
            var poco = new PocoA();
            var ts = "12:00:00";
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Time");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            _reader.Setup(d => d[0]).Returns(ts);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(TimeSpan.Parse(ts), poco.Item.Item.Time.Value);

            _reader.Setup(d => d.IsDBNull(0)).Returns(true);
            _reader.Setup(d => d[0]).Returns(null);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Null(poco.Item.Item.Time);
        }

        [Fact]
        public void enum_is_handled()
        {
            var poco = new PocoA();
            var ts = IsolationLevel.RepeatableRead;
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Enum");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            _reader.Setup(d => d[0]).Returns(ts.ToString());
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(ts, poco.Item.Item.Enum);

            _reader.Setup(d => d[0]).Returns((int)ts);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(ts, poco.Item.Item.Enum);
        }

        [Fact]
        public void culture_id_handled()
        {
            var poco = new PocoA();
            var ts = "en-US";
            _reader.Setup(d => d.GetName(0)).Returns("Item_Item_Info");
            _reader.Setup(d => d.IsDBNull(0)).Returns(false);
            _reader.Setup(d => d[0]).Returns(ts);
            _mapper.MapType(poco, _reader.Object, 0);
            Assert.Equal(new CultureInfo(ts), poco.Item.Item.Info); 
        }


       
        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}