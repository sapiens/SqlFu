//using System;
//using System.Linq;
//using CavemanTools.Model;
//using CavemanTools.Model.ValueObjects;
//using DomainBus.Tests;
//using FluentAssertions;
//using SqlFu.Builders.CreateTable;
//using SqlFu.Configuration;
//using SqlFu.Configuration.Internals;
//using SqlFu.Providers.SqlServer;
//using SqlFu.Tests.Providers;
//using SqlFu.Tests._Fakes;
//using Xunit;
//using Xunit.Abstractions;

//namespace SqlFu.Tests
//{

   
//    public class SomeOperations:IDisposable
//    {
//        private IDbFactory _getDb;

//        public SomeOperations(ITestOutputHelper x)
//        {
//            x.Logger();
//            SqlFuManager.ResetConfig();
//            SqlFuManager.Configure(c =>
//            {
//                c.AddProfile(SqlServer2012Provider.Instance, Setup.Connex);
//                c.AddSuffixTableConvention();
//            });
            
//            _getDb = SqlFuManager.GetDbFactory();

//            SetupTable();
//        }

//        private void SetupTable()
//        {

//            _getDb.Do(d =>
//            {
//                d.DropTable<Category>();
                
//                d.CreateTableFrom<SomePost>(cfg =>
//                {
//                    cfg.TableName("Posts");
               
//                    cfg.Column(col => col.Id, c =>
//                    {
//                        c.AutoIncrement();
//                    })
//                        .Column(col => col.Content, c =>
//                        {
//                            c.HasSize(75).Null();
//                        })
//                        .Column(c => c.State, c =>
//                        {
//                            c.DbTypeIs("nvarchar").HasSize("50").NotNull();
//                        })
//                        .Column(c => c.Title, c => c.HasSize(75))
//                        .Column(c=>c.CreatedOn,c=>c.DbTypeIs("datetime2"))
//                        ;

//                    cfg.PrimaryKey(c=>c.OnColumns(t=>t.Id));
//                    cfg.IfTableExists(IfTableExists.DropIt);
//                });
//                d.CreateTableFrom<Category>(cfg =>
//                {
//                    cfg.Column(c => c.Name, c => c.HasSize(70))
//                        .ForeignKeyFrom<SomePost>(
//                            c =>
//                                c.Columns(e => e.PostId)
//                                    .Reference(f => f.Id)
//                                    .OnDelete(ForeignKeyRelationCascade.Cascade));
//                });
//            });
//        }

//        [Fact]
//        public void crud()
//        {
//            //todo check for proper MapTo<> functionality. Map to an object that has additional properties, collections
//			_getDb.Do(db =>
//			{

//			    var ids = new Guid[] {Guid.Empty, Guid.Empty};
                
//			    db.Query(t => t.From<SomePost>().Where(d => ids.Contains(d.UId)).AllColumns());
//			    db.Query(t =>
//			    {
//			        var crit= t.From<SomePost>()
//			            .Where(d => ids.Contains(d.UId));
//                    if (true) crit=crit.And(d=>true);
//                    return crit.AllColumns();
                    
//			    });
             
//                var func = db.GetDbFunctions();

//                db.GetQueryValue(t => t.From<SomePost>().Where(d => d.State == SomeEnum.Last).AllColumns());
               
//                db.GetSingle(d => d.From<SomePost>().Select(c => new SomePost()));
//                var id=db.Insert(new SomePost() {Title = "my title"}).GetInsertedId<int>();
                
                
//                var all = db.GetQueryValue(f => f.From<SomePost>().Select(d => func.Count(d.Id)).MapTo<string>());
//                all.Should().Be("1");

//               var rez= db.Query(f => f.From<SomePost>().GroupBy(d=>d.CreatedOn).OrderBy(d=>d.CreatedOn).Limit(10,0).Select(d => new {Total = (int)func.Count(d.Id)}));

//                rez[0].Total.Should().Be(1);
//                db.Update<SomePost>().Set(d => d.Title, "changed").Where(d => d.Id == id).Execute().Should().Be(1);

//                db.UpdateFrom(o =>
//                {
//                    o.TableName = "Posts";
//                    o.DbSchema = "dbo";
//                }, d => d.FromData(new {Id = id, Title = "other"}).Ignore(i=>i.Id)).Where(d => d.Id == id).Execute();
               
//                var paged = db.FetchPaged<SomePost>(c => c.Sql("select * from Posts"),new Pagination());

//                var post = db.GetSingle<SomePost>(d => d.Id == id);
//                post.Title.Should().Be("other");

//                db.DeleteFrom<SomePost>(d => d.Id == id);

//            });
//        }

//        class IdTitle
//        {
//            public int Id { get; set; }
//            public string Name { get; set; } 
//        }

//        [Fact]
//        public void map_to_existing_type()
//        {
//            _getDb.Do(db =>
//            {
//                db.Query(b => b.From<SomePost>().Select(d => new IdTitle() {Id = d.Id, Name = d.Title}));

//            });
//        }


//        class EmailStore
//        {
//            public Email Data { get; set; }
//        }
//        [Fact]
//        public void map_value_object()
//        {
//            SqlFuManager.Config.MapValueObject<Email>(e=>e.Value,o=>new Email(o.ToString()));

//            //_getDb.Do(db =>
//            //{
                
//            //    db.CreateTableFrom<EmailStore>(t =>
//            //    {
//            //        t.Column(d => d.Data, c => c.DbTypeIs("varchar").HasSize(200));
//            //        t.IfTableExists(IfTableExists.DropIt);
//            //    });
//            //    db.Insert(new EmailStore() {Data = new Email("test@bla.com")});
//            //    db.DropTable<EmailStore>();
//            //});

//            SqlFuManager.Config.MapValueObject<SomeEnum>(d=>d.ToString());
//            _getDb.Do(db =>
//            {
//             //   db.Query<dynamic>("select * from Posts where state=@0", SomeEnum.First);

//                Func<SomeEnum>  en=()=> SomeEnum.First;
//               db.Query<SomePost>(f => f.From<SomePost>().Where(d => d.State == en()).AllColumns());
//            });
//        }

//        class ItemsRow
//        {
//            public string Id { get; set; } 
//        }

//        [Fact]
//        public void row_suffix_convention()
//        {
//            _getDb.Do(db =>
//            {
//                db.CreateTableFrom<ItemsRow>(c => { c.IfTableExists(IfTableExists.DropIt); });
//                db.TableExists("Items").Should().BeTrue();
//            });

//        }

//        public void Dispose()
//        {
         
//        }
//    }
//}