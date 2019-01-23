#Welcome to SqlFu

SqlFu is a **_flexibile_** data mapper (aka micro-ORM) for .Net Core 2 and .Net 4.6+ . Apache 2.0 license.

Latest version: [4.2.0](https://github.com/sapiens/SqlFu/wiki/ChangeLog) 
  
## Features
* Think Ado.Net on steroids with the addition of a strongly typed query builder (not LINQ).
* **Designed to increase developer productivity** while remaining simple to use and fast
* Runs on any platform implementing NetStandard 2.0
* All helpers have sync/async versions
* Dependency Injection support for working with multiple databases/providers in the same app
* Implicit transient errors resilience
* Great for CRUD apps and for maintaining and querying the read model of CQRS apps
* Supports: SqlServer 2012+ (Azure included), Sqlite.


## How SqlFu should be used

It's important to understand that SqlFu is **NOT** a (light) ORM. While an ORM abstracts sql and gives us the illusion of working with a 'object database', SqlFu maps data from a query result to a POCO and provides helpers which use POCOs as a data source. Simply put, an object in SqlFu is a data _source_ or _destination_. There are no relational table to object and back mappings that magically generate sql. 

The strongly typed helpers or sql builders are just that: a specialised string builder which uses expressions, there is no Linq involved. In SqlFu we think Sql but we write it mostly in C#. Think of SqlFu as a **powerful facade for Ado.Net**.

Usually we use a POCO (a defined or anonymous type) to represent a table or a view. SqlFu helpers are flexible enough for most one table queries, but if you need to join tables, you should either write the sql as string (not really recommended) or create a db view (recommended) or a stored procedure.

SqlFu is designed to be used in a cloud environment and it works great inside DDD/CQRS apps or simple CRUD apps.

### Note for contributors

Please create your pull requests to target the "v4-devel" branch. "Master" is only for released code. Thank you.



##Usage

### New!!!

```csharp

//quick query using the default connection
var name=SqlFuManager.QueryOver<MyObject>().Select(d => d.FirstName,criteria: d => d.FirstName == "John").GetValue();


//quick update
//"schema.table" is implicit converted to `TableName` instance
_db.UpdateFrom(new {Name="Foo"},"myTable").Where(new{Id=2}).Execute();

```

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
               
			   //set how a certain type should be treated at writing/reading. Useful for enums, DateTime or value objects
			    c.WhenType<ArticleType>().WriteAs(a => a.ToString());

               //set the table name to be used when dealing with this POCO. 
             		   
               c.ConfigureTableForPoco<MyPoco>(info=>
											{
												info.Table=new TableName("my_pocos");
												
												//new in ver. 4.0.0 - additional info used by helpers
												
												//Any table name used by helpers will use it by default
												c.SetDefaultDbSchema("foo");
												
												//the Insert helper uses it
												 d.Property(f => f.Id).IsAutoincremented();
												 
												 //properties will be always be ignored
												 d.IgnoreProperties(f=>f.Ignored);
												 
												 //use it when you want to convert value just before writing to the db
												 // store an enum as a string instead of the default int
												 d.Property(f => f.Category)
												 .BeforeWritingUseConverter(t => t.ToString())
												 //the column name in the db is 'Categ'
												 .MapToColumn("Categ");


											}
										);
               
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
### Working with multiple databases/providers

Let's assume I need 2 connections in my app: one for db "Main", other for db "History". First we we declare specific interfaces that will be used by the objects which need db access, then add the profiles for each db.
SqlFu automatically generates concrete classes deriving from `DbFactory` and implementing the interfaces. All factories are singletons.

```csharp
 public interface IMainDb:IDbFactory
{
    
}

public interface IHistoryDb:IDbFactory
{
    
}

//register the db profiles
 SqlFuManager.Configure(c =>
 {
     //default profile
     c.AddProfile<IMainDB>(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),MainConnex);              
     //history profile
     c.AddProfile<IHistoryDb>(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),HistoryConnex,"history");              
 });


//get main db factory singleton
var main=  SqlFuManager.GetDbFactory<IMainDb>();

//get history db singleton
var history=SqlFuManager.GetDbFactory<IHistoryDb>();

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

### Transient Errors Resilience

It's a common scenario, especially using a cloud based db like Azure Sql, to reach connections limit or the opening of a connection to timeout. 
SqlFu automatically employs a simple strategy to retry the operation a number of times. You can configure it like this:

```csharp
	SqlFuManager.Configure(c=>
					{
						//other options are available too
						c.ConfigureDefaultTransientResilience(f=>f.MaxRetries=5);

						//if you want to implement your own strategy, you need to create a class implementing `IRetryOnTransientErrorsStrategy`
						c.TransientErrorsStrategyFactory=()=>new MyStrategy();
					});
	
```


### CRUD Helpers

Almost all helpers have async counterparts

```csharp

DbConnection _db=dbFactory.Create();

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

//Ignores unique key constraints. Useful when updating read models
_db.InsertIgnore(new User()
            {
                FirstName = "John",
                LastName = "Doe"
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

### Queries

SqlFu features a quite powerful and flexible query builder that you can use to query one table/view (use views or sprocs when you need joins).
Note that it can be useful or a big PITA, in doubt go for the simplest thing.

```csharp

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
 _db.WithSql((q=>q.From<User>().SelectAll())
	.ProcessEachRow(user=>{ 
		   user.Name=user.Name.ToUpper();
		   return true;//continue processing
		   return false;//query ends here, no other results are read/mapped
		 }).Execute();
 
 //query using interpolated strings . Variables are converted into query params
 _db.SqlTo<User>($"select * from users where id ={id}").GetRows();          
_db.SqlTo<User>(q=>q.Append("select * from users").AppendIf(d=>id>0,$" where id={id}")).GetRows()

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

### SProc Support

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