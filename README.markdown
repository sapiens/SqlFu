#Welcome to SqlFu

SqlFu is a **_versatile_** object mapper (aka micro-ORM)  for .Net 4.6+ (.Net Core included).  SqlFu uses Apache 2.0 license.

Latest version: [3.0.0-beta-1](https://github.com/sapiens/SqlFu/wiki/ChangeLog)

## Features
* Versatility 
* Performance
* Fully async extension methods and helpers
* Suport for working with multiple databases/providers in the same app
* Transient errors resilience
* Support for: SqlServer 2012+ (Azure included). TBA: Sqlite, Postgres, MySql

**Version 3 is not compatible with previous versions**

## How SqlFu should be used

It's important to understand that SqlFu is **NOT** a (light) ORM. While an ORM abstracts sql and gives us the illusion of working with a 'object database', SqlFu maps data from a query result to a POCO and provides helpers which use POCOs as a data source. Simply put, an object in SqlFu is a data _source_ or _destination_. There are no relational table to object and back mappings that magically generate sql. 

The strongly typed helpers or sql builders are just that: a specialised string builder which uses expressions, there is no Linq involved. In SqlFu we think Sql but we write it mostly in C#. Think of SqlFu as a powerful facade for Ado.Net.

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
var main=  SqlFuManager.GetDbFactory<IMainDb>();

//get history db
var history=SqlFuManager.GetDbFactory<IHistoryDb>("history");

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

getDb.HandleTransientErrors(db=>{
 /* do stuff */
 });
 
return getDb.HandleTransientErrors(db=> /* some query */);
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

SqlFu features a quite powerful and flexible query builder that you can use to query one table/view
```csharp


```

**Notes**
* The convention is that every extension method starting with `Query` uses the strongly typed sql builder

### Db Tools

