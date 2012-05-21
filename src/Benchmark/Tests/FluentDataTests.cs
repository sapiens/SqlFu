using System;
using CavemanTools.Testing;
using FluentData;
using Tests;
using SqlFu;

namespace Benchmark.Tests
{
    public class FluentDataTests:PerformanceTests
    {
        private IDbContext _db;

        private const string Name = "FluentData";
        public FluentDataTests()
        {
            _db = new DbContext().ConnectionStringName(Config.ConnectionName,
                DbProviderTypes.SqlServer);
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(d=>
                       {
                           _db.Sql(@"select * from sfPosts
				where id = @0", 5).QuerySingle<sfPosts>();
                       },Name);
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _db.Sql(@"select * from sfPosts
				where id = @0", 5).QuerySingle();
            }, Name);
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _db.Sql(@"select top 10 * from sfPosts
				where id > @0", 5).Query<sfPosts>();
            }, Name);
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _db.Sql(@"select top 10 * from sfPosts
				where id > @0", 5).Query();
            }, Name);
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(d => { throw new NotSupportedException("No implicit pagination support"); }, Name);
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(d =>
                       {
                           _db.Sql(@"select authorid from sfPosts
				where id = @0", 5).QueryValue<int>();
                       }, Name);
                     
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.IgnoreIfAutoMapFails.Sql("select *,id as Author_Id,title as Author_Name from sfposts where id=@0 order by id", 5).Query<PostViewModel>();
            }, Name);
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            _db.OnExecuting(d => d.Command.FormatCommand());
            bc.Add(d=>
                       {
                           //_db.Insert<sfPosts>("sfPosts", p).Execute();
                           throw new NotSupportedException();
                       },Name);
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _db.Update("sfPosts",new{Id=3,Title="updated"} ).Where("id",3).AutoMap(k=>k.Id).Execute();
            }, Name);
        }
    }
}