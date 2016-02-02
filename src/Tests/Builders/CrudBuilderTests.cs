using System;
using System.Data.Common;
using System.Threading.Tasks;
using SqlFu;
using Tests.Providers;
using Xunit;


namespace Tests.Builders
{
    public class CrudBuilderTests:IDisposable
    {
        private DbConnection _db;
   

        public CrudBuilderTests()
        {
            //var mapping = new BuilderMapping();
            //SqlFuManager.Config.AddTableMapping(mapping);
         
            //_db = Setup.GetConnection();
          
            //_db.CreateTable<SomePost>();
            
        }

        [Fact]
        public async Task delete_from_subquery()
        {
            using (var t = _db.BeginTransaction())
            {

                //await _db.InsertAsyncInto<SomePost>(new { Title = "hi" });
                //var sub = _db.From<SomePost>().Select(d => d.Id).Where(d => d.Id < 30);
                //var rows = await _db.DeleteAsync<SomePost>(d =>d.Where(tb=>tb.Id).In(sub));
                //rows.Should().Be(1);                
            }
        }

        [Fact]
        public async Task insert_returns_id()
        {
            using (var t = _db.BeginTransaction())
            {
              
                //var data = new { State = SomeEnum.First, Title = "test" };
                //// var data = new SomePost(){State = SomeEnum.First,Title = "test"};
                //var i = await _db.InsertAsyncInto<SomePost>(data);
                //i.GetInsertedId<int>().Should().Be(1);
               
            }

        }

        [Fact]
        public async Task delete()
        {
            using (var t = _db.BeginTransaction())
            {
               
                //await _db.InsertAsyncInto<SomePost>(new {Title = "hi"});
                //var rows=await _db.DeleteAsyncFrom<SomePost>(d=>d.Id>30);
                //rows.Should().Be(0);
                //rows=await _db.DeleteAsyncFrom<SomePost>();
                //rows.Should().Be(1);
            }
        
        }

        [Fact]
        public async Task update_with_fluent()
        {
            using (var t = _db.BeginTransaction())
            {
               
                //var id=await _db.InsertAsyncInto<SomePost>(new {Title = "hi",Content=" bla DLDda"});
                //var row=await _db.UpdateAsync<SomePost>(d => 
                //    d.Set(c => c.Title, "bla")
                //    .Set(c=>c.Content,c=>c.Title.ToLower())
                 
                //    .Where(c=>c.Id==id.GetInsertedId<int>())
                //    );
                //row.Should().Be(1);
            }
        
        }
        
        [Fact]
        public async Task update_with_object()
        {
            using (var t = _db.BeginTransaction())
            {
               
                //var id=await _db.InsertAsyncInto<SomePost>(new {Title = "hi",Content=" bla DLDda"});
                //var row = await _db.UpdateAsync<SomePost>(new {Title="bla" },c=>c.Id==id.GetInsertedId<int>());
                //row.Should().Be(1);
            }
        
        }

        public object Count()
        {
            return null;
        }
       
        public int Count<T>(T column,int id)
        {
            return 0;
        }


        public void Dispose()
        {
            _db.DropTable<SomePost>();
            _db.Dispose();
        }
    }


    //class Count
    //{
    //    private readonly ITableInfoFactory _factory;
    //    private readonly IEscapeIdentifier _provider;

    //    public Count(ITableInfoFactory factory,IEscapeIdentifier provider)
    //    {
    //        _factory = factory;
    //        _provider = provider;
    //    }

    //    public string WriteMethodCall(MethodCallExpression method,ParametersManager paramsManager)
    //    {
    //        if (method.Arguments.Count == 0)
    //        {
        
    //            return " count(*)";
    //        }
    //        var column = method.Arguments[0] as MemberExpression;
    //        var info = _factory.GetColumnName(column, _provider);
    //        return " count({0})".ToFormat(info);
    //    }
    //}
}