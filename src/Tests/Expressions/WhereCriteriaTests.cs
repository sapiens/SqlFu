using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Moq;
using SqlFu;
using SqlFu.Expressions;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Expressions
{
    

    public class WhereCriteriaTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private ExpressionWriter _w;
        private ParametersManager _pm;
        private readonly Mock<IDbProviderExpressionHelper> _fRm;

        public WhereCriteriaTests()
        {
            _sb = new StringBuilder();
            _pm = new ParametersManager();
            _fRm = new Mock<IDbProviderExpressionHelper>();
            _fRm.Setup(d => d.EscapeName(It.IsAny<string>())).Returns((string v) => SqlServerProvider.EscapeIdentifier(v));
            _fRm.Setup(d => d.FormatBoolean(It.IsAny<bool>())).Returns((bool v) => v==true?"1":"0");
            _w = new ExpressionWriter(_sb,_fRm.Object,_pm);        
        }

        int Id()
        {
            return 2;
        }

        [Fact]
        public void simple_equality()
        {
            Expression<Func<PostViewModel, bool>> data = p => p.Id==23 ;
            _w.Write(data);
            Assert.Equal("([Id] = 23)",_sb.ToString());            
        }


        [Fact]
        public void id_greater_than_12_and_less_than_24()
        {
            Expression<Func<PostViewModel, bool>> data = p => p.Id > 12 && p.Id<24;
            _w.Write(data);
            Assert.Equal("(([Id] > 12) and ([Id] < 24))", _sb.ToString());            
        }

        [Fact]
        public void id_equals_field_or_title_is_null()
        {
            var d = 12;
            Expression<Func<PostViewModel, bool>> data = p => p.Id == d || p.Title==null;
            _w.Write(data);
            Assert.Equal("(([Id] = @0) or ([Title] is null))", _sb.ToString());            
            Assert.Equal(12,_pm.ToArray().First());
        }

        [Fact]
        public void combne_and_or_filed_and_method()
        {
            var d = 12;
            Expression<Func<PostViewModel, bool>> data = p => p.Id == d && (p.Title == "f" || p.Id==Id());
            _w.Write(data);
            Assert.Equal("(([Id] = @0) and (([Title] = 'f') or ([Id] = @1)))",_sb.ToString());
            Assert.Equal(2,_pm.ToArray().Count());
        }

        [Fact]
        public void id_not_equal_to_value()
        {
            Expression<Func<PostViewModel, bool>> data = p => p.Id !=12;
            _w.Write(data);
            Assert.Equal("([Id] <> 12)",_sb.ToString());
        }


        [Fact]
        public void enum_handling()
        {
            Expression<Func<PostViewModel, bool>> data = p => p.Type==PostType.Page;
            _w.Write(data);
            Assert.Equal("([Type] = 1)",_sb.ToString());
        }

        [Fact]
        public void id_and_isActive_not_true()
        {
            Expression<Func<Post, bool>> data = p => p.Id==23 && !p.IsActive;
            _w.Write(data);
            Assert.Equal("(([Id] = 23) and ([IsActive] = 0))", _sb.ToString());
        }
        [Fact]
        public void id_and_isActive_is_true()
        {
            Expression<Func<Post, bool>> data = p => p.Id==23 && p.IsActive;
            _w.Write(data);
            Assert.Equal("(([Id] = 23) and ([IsActive] = 1))", _sb.ToString());
        }

        [Fact]
        public void isActive_is_true_and_id()
        {
            Expression<Func<Post, bool>> data = p =>p.IsActive && p.Id == 23;
            _w.Write(data);
            Assert.Equal("(([IsActive] = 1) and ([Id] = 23))", _sb.ToString());
        }
        
        [Fact]
        public void isActive_is_true()
        {
            Expression<Func<Post, bool>> data = p => p.IsActive;
            _w.Write(data);
            Assert.Equal("([IsActive] = 1)", _sb.ToString());
        }
        
        [Fact]
        public void isActive_is_false()
        {
            Expression<Func<Post, bool>> data = p => !p.IsActive;
            _w.Write(data);
            Assert.Equal("([IsActive] = 0)", _sb.ToString());
        }

        [Fact]
        public void nullable_equality()
        {
            Expression<Func<Post, bool>> data = p => p.TopicId==2;
            _w.Write(data);
            Assert.Equal("([TopicId] = 2)", _sb.ToString());
        }

        [Fact]
        public void contains_id_from_extension_method()
        {
            Expression<Func<Post, bool>> data = p => new[] { 1, 3 }.Contains(p.Id);
            _w.Write(data);
            Assert.Equal("[Id] in (@0)", _sb.ToString());
            Assert.Equal(2, _pm.ToArray().First().As<IList>().Count);
            Assert.True(_pm.ToArray().First().IsListParam());
        }

        [Fact]
        public void contains_id_from_list()
        {
            var bla = new List<int>();
            bla.Add(1);
            bla.Add(2);
            Expression<Func<Post, bool>> data = p => bla.Contains(p.Id);
            _w.Write(data);
            Assert.Equal("[Id] in (@0)",_sb.ToString());
            Assert.Equal(2,_pm.ToArray().First().As<IList>().Count);
            Assert.True(_pm.ToArray().First().IsListParam());           
        }

        [Fact]
        public void title_start_with_a()
        {
            Expression<Func<Post, bool>> data = p => p.Title.StartsWith("a");
            _w.Write(data);
            Assert.Equal("[Title] like @0", _sb.ToString());
            Assert.Equal("a%", _pm.ToArray()[0]);
        }
        
        [Fact]
        public void title_end_with_a()
        {
            Expression<Func<Post, bool>> data = p => p.Title.EndsWith("a");
            _w.Write(data);
            Assert.Equal("[Title] like @0", _sb.ToString());
            Assert.Equal("%a", _pm.ToArray()[0]);
        }

        [Fact]
        public void when_string_contains_quotes_they_should_be_escaped()
        {
            Expression<Func<Post, bool>> data = p => p.Title.Contains("don't cry for me argentina");
            _w.Write(data);
            Assert.Equal("[Title] like @0",_sb.ToString());
            Assert.Equal("%don't cry for me argentina%",_pm.ToArray()[0]);
        }

        //[Fact]
        //public void title_contains_bla()
        //{
        //    Expression<Func<Post, bool>> data = p => p.Title.Contains("bla");
        //    _w.Write(data);
        //    Assert.Equal("[Title] like '%bla%'", _sb.ToString());
        //}

        [Fact]
        public void title_length_less_than_3()
        {
            Expression<Func<Post, bool>> data = p => p.Title.Length<3;
            _fRm.Setup(d => d.Length(It.IsAny<string>())).Returns("Title");
            _w.Write(data);
            _fRm.Verify(d=>d.Length("Title"));            
        }
        
        [Fact]
        public void title_substring()
        {
            Expression<Func<Post, bool>> data = p => p.Title.Substring(0,3)=="bla";
            _fRm.Setup(d => d.Substring("Title",0,3)).Returns("Title").Verifiable();
            _w.Write(data);
            _fRm.Verify();
        }
        
        [Fact]
        public void datetime_year()
        {
            Expression<Func<Post, bool>> data = p => p.CreatedOn.Year==DateTimeOffset.UtcNow.Year;
            _fRm.Setup(d => d.Year("CreatedOn")).Returns("Year(CreatedOn)").Verifiable();
            _w.Write(data);
            _fRm.Verify();
        }
        
        [Fact]
        public void datetime_day()
        {
            Expression<Func<Post, bool>> data = p => p.CreatedOn.Day==12;
            _fRm.Setup(d => d.Day("CreatedOn")).Returns("Day(CreatedOn)").Verifiable();
            _w.Write(data);
            _fRm.Verify();
        }

        [Fact]
        public void some_arithmetics()
        {
            Expression<Func<Post, bool>> data = p => p.Id+4*p.Id==p.AuthorId;
            _w.Write(data);
            Assert.Equal("(([Id] + (4 * [Id])) = [AuthorId])",_sb.ToString());
        }

        [Fact]
        public void property_is_null()
        {
            Expression<Func<Post, bool>> data = p => p.TopicId==null;
            _w.Write(data);
            Assert.Equal("([TopicId] is null)",_sb.ToString());
        }

        [Fact]
        public void propety_is_not_null()
        {
            Expression<Func<Post, bool>> data = p => p.TopicId != null;
            _w.Write(data);
            Assert.Equal("([TopicId] is not null)", _sb.ToString());
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Write(_sb.ToString());
        }
    }
}