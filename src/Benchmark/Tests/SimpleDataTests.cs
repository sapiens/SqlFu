using System;
using System.Data.SqlClient;
using System.Linq;
using CavemanTools.Testing;
using Simple.Data;
using Simple.Data.SqlServer;
using SqlFu;
using Tests;

namespace Benchmark.Tests
{
    public class SimpleDataTests:PerformanceTests,IDisposable
    {
        private dynamic _db;
        private SqlConnection _cnx;

        public SimpleDataTests()
        {
            _db = Database.OpenConnection(Config.Connex);
            _cnx = new SqlConnection(Config.Connex);
            _cnx.Open();
            _db.UseSharedConnection(_cnx);

        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(id=>
                       {
                           throw new NotSupportedException();
                       },"SimpleData");
            
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.sfPosts.Get(5);
            }, "SimpleData dynamic");
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(id=>
                       {
                           throw new NotSupportedException();
                       }, "SimpleData");
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.sfPosts.FindAll(_db.sfPosts.Id>5).ToArray();
            }, "SimpleData");
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(id =>
                {
                    _db.sfPosts.FindAll(_db.sfPosts.Id > 5).Skip(0).Take(10).ToArray();
                }, "SimpleData");
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                //_db.sfPosts.FindById(5).Select(_db["sfPosts"]["Title"]).FirstOrDefault().ToScalar<string>();
                _db.sfPosts.Get(5).ToScalar<int>();
            }, "SimpleData scalar");
                   
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                throw new NotSupportedException();
            }, "SimpleData");
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(id =>
                       {
                           _db.sfPosts.Insert(p);
                       }, "SimpleData");
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.sfPosts.UpdateById(new { Id = 3, Title = "updated" });
            }, "SimpleData");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _db.StopUsingSharedConnection();
            _cnx.Dispose();
        }
    }
}