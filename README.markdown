#Welcome to SqlFu

SqlFu is a **_versatile_** data mapper (aka micro-ORM)  for .Net 4.6+ and .Net Core.  SqlFu uses Apache 2.0 license.

Latest version: [3.3.5](https://github.com/sapiens/SqlFu/wiki/ChangeLog) 
 
 **new!!** [How to add builder support for any sql functions](https://github.com/sapiens/SqlFu/wiki/Adding-support-for-sql-functions)
 
 [Docs](https://github.com/sapiens/SqlFu/tree/v2) for version v2.

## Features
* Versatility 
* Performance
* CoreClr support
* Fully async extension methods and helpers
* DDL tools
* Suport for working with multiple databases/providers in the same app
* Transient errors resilience
* Great for maintaining and querying the read model of CQRS apps
* Support for: SqlServer 2012+ (Azure included), Sqlite. TBA: Postgres, MySql

**Version 3 is not compatible with previous versions**

## How SqlFu should be used

It's important to understand that SqlFu is **NOT** a (light) ORM. While an ORM abstracts sql and gives us the illusion of working with a 'object database', SqlFu maps data from a query result to a POCO and provides helpers which use POCOs as a data source. Simply put, an object in SqlFu is a data _source_ or _destination_. There are no relational table to object and back mappings that magically generate sql. 

The strongly typed helpers or sql builders are just that: a specialised string builder which uses expressions, there is no Linq involved. In SqlFu we think Sql but we write it mostly in C#. Think of SqlFu as a **powerful facade for Ado.Net**.

Usually we use a POCO (a defined or anonymous type) to represent a table or a view. SqlFu helpers are flexible enough for most one table queries, but if you need to join tables, you should either write the sql as string (not really recommended) or create a db view (recommended) or a stored procedure.

SqlFu is designed to be used in a cloud environment and it works great inside DDD/CQRS apps or simple CRUD apps.

### Note for contributors

Please create your pull requests to target the "devel" branch. "Master" is only for released code. Thank you.



##Usage

### Config options
```csharp
LogManager.OutputToTrace();
 SqlFuManager.Configure(c =>
            {
               //add the default profile (name is 'default')
               c.AddProfile(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),cnx_string);              
               
               //add named profile
               c.AddProfile(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),cnx_string,"other");              
               
               //register a type converter for query purposes, obj -> Email
               c.RegisterConverter(val=>new Email(val.ToString()));
               
               //register a custom (manual) mapper
               c.CustomMappers.Register(reader=> new MyPoco(){ /* init from DbDataReader */});
               
               //set the table name to be used when dealing with this POCO. 
               //You can also set the name when using the helper to create a table, or in the helper options when using a helper
               c.ConfigureTableForPoco<MyPoco>(info=>info.Table=new TableName("my_pocos"));
               
              //register a naming convention
              c.AddNamingConvention(predicate,type=> new TableName(type.Fullname));
              
              //used a predefined convention. PostsItem is considered to 'represent' the table/view "Posts"
              c.AddSuffixTableConvention(suffix:"Item");
              
              //custom logging
              c.OnException = (cmd,ex)=> Logger.Error(cmd.FormatCommand(),ex);
            });

````
**Notes**
* You need at least one profile configured
* Each profile is a combination of provider/connection string and it allows to use multiple databases
* To support CoreClr, each provider needs a DBConnection factory injected. This means that when running on coreclr you need to also install the "System.Data.SqlClient" package. SqlFu is decoupled from a specific db provider.

### Get Connection

Most of the time you'll need to inject a db connection factory into your Repository/DAO/Query object . It's **always** better to do that instead of injecting a DbConnection. `IDbFactory` is the predefined factory abstraction in SqlFu.

```csharp

public class MyRepository
{
    public MyRepository(IDbFactory getDb){}
    
    public void DoStuff()
    {
        using(var db=_getDb.Create())
        {
            //use db connection
        }
    }
}

//gets factory for the default profile
var factory=SqlFuManager.GetDbFactory();

//get a specific profile
var factory=SqlFuManager.GetDbFactory("other");

var repo=new MyRepository(factory);

```

Let's assume I need 2 connections in my app: one for db "Main", other for db "History". First we add the profiles for each db, then we declare specific interfaces that will be used by the objects which need db access, then the factory classes.

```csharp
//register the db profiles
 SqlFuManager.Configure(c =>
 {
     //default profile
     c.AddProfile(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),MainConnex);              
     //history profile
     c.AddProfile(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),HistoryConnex,"history");              
 });

 public interface IMainDb:IDbFactory
{
    
}

public interface IHistoryDb:IDbFactory
{
    
}

 public class MainDb : DbFactory, IMainDb
{
    
}

public class HistoryDb : DbFactory, IHistoryDb
{
    
}

//get main db factory
var main=  SqlFuManager.GetDbFactory<MainDb>();

//get history db
var history=SqlFuManager.GetDbFactory<HistoryDb>("history");

//register into DI Container to be injected in a service
//autofac
var cb=new ContainerBuilder();
cb.Register(c=>main).As<IMainDb>().SingleInstance();
cb.Register(c=>history).As<IHistoryDb>().SingleInstance();

//uses both main db and history db
public class MyService
{
    public MyService(IMainDb db,IHistoryDb) {}
}

```
**Notes**
* A custom db factory **must** have a public parameterless constructor.
* Db factories should be registered/treated as singletons
* For each database you use, you declare an interface inheriting `IDbFactory` and a type that implements the interface _and_ extends `DbFactory`.


### Transient Errors Resilience

It's a common scenario, especially using a cloud based db like Azure Sql, to reach connections limit or the opening of a connection to timeout. Those are transient errors and SqlFu has some support in handling them. Basically, when one of the above situations is detected, the db operation is retried for a number of times. This means you get an exception only if, after all retries, the error still persists. 

```csharp

IDbFactory getDb;

getDb.RetryOnTransientErrorAsync(cancel,async db=>{
 var items=await db.WithSql(q=>q.From<MyTable>().Where(d=>d.Id==2).SelectAll(useAsterisk:true)).GetRowsAsync();
 return items;
 });
 
```

### CRUD Helpers

```csharp
DbConnection _db;

//insert
_db.Insert(new User()
            {
                FirstName = "John",
                LastName = "Doe"
            });

//insert with options
_db.Insert(new 
            {
                FirstName = "John",
                LastName = "Doe",
                Bla=0
            }, cf =>
            {
                cf.SetTableName("mytable");
                cf.Ignore(d=>d.Bla);
            });

//update
_db.Update<User>()
.Set(c=>c.FirstName,"John").Set(c=>c.Posts,c.Posts+1)
.Where(c=>c.Id==userId)
.Execute();

//update from anonymous
 _db.UpdateFrom(
                q => q.Data(new { Firstname = "John3", Id = 3 }).Ignore(d => d.Id)
                ,o => o.SetTableName("users")
                )
                .Where(d => d.Firstname == "John")
                .Execute();

//delete
_db.DeleteFrom<User>(d=>d.Id==id);
_db.DeleteFromAnonymous(
    new {Category = ""}
    , opt => opt.SetTableName("users")
    , d => d.Category == Type.Page.ToString());

```

### SPoc Support

```csharp
//execute a sproc
 var result = _db.ExecuteSProc(s =>
              {
                  s.ProcName = "spTest";
                  s.Arguments = new { id = 47, _pout = "" };
              });
result.ReturnValue.Should().Be(100);
string pout = r.OutputValues.pout;

//execute a sproc which returns a result set
var res = _db.QuerySProc<MyPoco>("spTest", new { id = 46, _pout = "" });
res.ReturnValue.Should().Be(100);
//do something with the result set List<MyPoco>
return r.Result;
  
```

**Notes**
* Output arguments are identified through the `_` prefix.

### Queries

SqlFu features a quite powerful and flexible query builder that you can use to query one table/view (use views or sprocs when you need joins).
```csharp

//starting with ver. 3.3.0
//alternative syntax
_db.WithSql(q => q.From<User>()
            .Where(d=>d.Id==id && !d.IsActive)
            .OrderByIf(c=>input.ShouldSort,d=>d.Name)
            .SelectAll())
    .GetRows();

//a big unrealistic query to showcase the builder capabilities
var names=new[]{"john","mary"};
_db.QueryAs(q => q.From<User>()
            .Where(d=>d.Id==id && !d.IsActive)
            .And(d=>d.InjectSql("Posts=@no",new {no=100}))
            .Or(d=>d.FirstName.HasValueIn(names))
            .Or(d=>names.Contains(d.FirstName))
            .GroupBy(d => d.Category)
            .Limit(10)
            .Select(d => new {
                             d.Category
                             , total= d.Sum(d.Posts * d.Count(d.Id))})
                );
 //you can use pocos to create the sql but map the result to a different poco
 _db.QueryAs(q => q.From<User>().SelectAll().MapTo<OtherPoco>());
 
//returns one row only
 _db.QueryRow(q=>q.From<User>().SelectAll());
 _db.WithSql(q=>q.From<User>().SelectAll()).GetFirstRow();

//returns one value
 _db.QueryValue(q=>q.From<User>().Select(d=>d.Id));
 _db.WithSql(q=>q.From<User>().Select(d=>d.Id)).GetValue();
 
 //returns a List<int>
 _db.QueryAs(q=>q.From<User>().Select(d=>d.Id));
 
 //process a result set row by row. Useful when dealing with a big result set
 _db.QueryAndProcess(q=>q.From<User>().SelectAll(),user=>{ 
   user.Name=user.Name.ToUpper();
   return true;//continue processing
   return false;//query ends here, no other results are read/mapped
 });
 _db.WithSql((q=>q.From<User>().SelectAll()).ProcessEachRow(user=>{ 
   user.Name=user.Name.ToUpper();
   return true;//continue processing
   return false;//query ends here, no other results are read/mapped
 }).Execute();
 
 
 //do a paged query, useful for pagination. Here we request page 2 with 30 results per page
 var result=_db.QueryPaged<User>(q=>q.From<User>.SelectAll(),new Pagination(page:2,pageSize:30));
 //total existing users
 result.Count
 
 //result set with 30 users
 result.Items
 
 //execute some sql
 _db.Execute($"delete from {_db.GetTableName<User>()} where Id=@0",userId);
```

**Notes**
* The convention is that every extension method starting with `Query` uses the strongly typed sql builder.
* `InjectSql` is used only on the epxression parameter; as the name implies, it allows you to inject raw sql in the builder.
* `HasValueIn` is for `column in (values)` sql.
* Any IEnumerable variable can use `Contains(column)` to generate `column in (values)` sql;
* `Select` is about specifying the sql column and an implicit mapping to the projection, however `MapTo` applies after the sql has been built and the query executed.
* Sql functions should be used only on the expression parameter. Supported functions are: Sum, Count, Avg, Floor, Ceiling, Min, Max,Concat, Round.
* String methods/properties support: Contains, Length, StartsWith, EndsWith, ToUpper, ToLower.

### Db Tools

```csharp
_db.DropTable<User>();
_db.DropTable("users");

_db.Truncate<User>();

if (_db.TableExists<User>()){}

//creates table starting from POCO. This is not poco to table mapping, just a fluent builder to generate 'create table' command
_db.CreateTableFrom<User>(cf=>{
              cf.DropIfExists()
                .TableName("users")
                .Column(t => t.Id, c => c.AutoIncrement())
                .ColumnSize(c=>c.FirstName,150)
                .ColumnSize(c=>c.LastName,150)
                .Column(d=>d.Category,c=>c
                                        .HasDbType(SqlServerType.Varchar)
                                        .HasSize(10)
                                        .HasDefaultValue(Type.Page.ToString()))
             
                .PrimaryKey(pk=>pk.OnColumns(d=>d.Id))
                .Index(ix=>ix.OnColumns(d=>d.FirstName).Unique().WithOptions("nonclustered"));
                ;
});

```

**Notes**
* SqlServer type `rowversion` should be a byte[8] property of the POCO.
* By default enums are considered ints, in order to store strings, the POCO property must be string. This limitation is because of how the c# compiler treats enums in an Expression (it always converts it to int).
* But when mapping a result to POCO, `int` and `string` are automatically maped to enum without any configuration.

#### The strongly typed table creator

In order to create the needed tables in an organised manner (ex: part of a component which needs those tables) you can use the `ATypedStorageCreator<>` base class (one for each table). This is actual code from another library
```csharp
 public class UniqueStorageCreator : ATypedStorageCreator<UniqueStoreRow>
    {
        public const string DefaultTableName = "uniques";
        public const string DefaultSchema = "";

        public UniqueStorageCreator(IDbFactory db) : base(db)
        {
        }

       
        protected override void Configure(IConfigureTable<UniqueStoreRow> cfg)
        {
            cfg.Column(d => d.Scope, c => c.HasDbType("char").HasSize(32).NotNull())
                .Column(d => d.Aspect, c => c.HasDbType("char").HasSize(32).NotNull())
                .Column(d => d.Value, c => c.HasDbType("char").HasSize(32).NotNull())
                .Column(d => d.Bucket, c => c.HasDbType("char").HasSize(32).NotNull())
                .Index(i => i.OnColumns(c=>c.Bucket,c => c.Scope,c=>c.Aspect,c=>c.Value).Unique())
                .Index(d=>d.OnColumns(c=>c.EntityId))
                .HandleExisting(HandleExistingTable);
        }
    }
    
    //usage
    new UniqueStorageCreator(factory).WithTableName(name,schema).IfExists(TableExistsAction.DropIt).Create();
  ```
**Notes**

This has nothing to do with migrations support, SqlFu doesn't support schema migrations anymore, it's just a convenient way to create tables. Another way is to register all these creators in a DI Container then resolve `IEnumerable<ICreateStorage>` and then `storages.ForEach(s=>s.Create())`. This allows you to add new table creator classes at any time. Great for development where the db schema is not stable.
