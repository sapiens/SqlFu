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

### Config
```csharp
LogManager.OutputToTrace();
 SqlFuManager.Configure(c =>
            {
               //add default profile (name is 'default')
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


### Common Usage

Starting with version 2.0.0 (.Net 4.5 only) SqlFu adds async queries support. The async methods follow the "Async" sufix convention (e.g Query => QueryAsync)

```csharp

//usual stuff
db.Get<Post>(id)

//1.3.0+
db.Get<Post>(p=>p.Id==12 && p.Title.StartsWith("A"))

db.Query<Post>("select * from posts where id=@0",1);
db.Query<dynamic>("select * from posts where id=@0",1);// dynamic objects are read only
db.Query<Post>(p=>p.CreatedAt.Year==2013 && p.IsActive)


//you can pass ordinal params or anonymous objects
db.Query<Post>("select * from posts where id=@id",new{id=1});

db.GetValue<int>("select count(*) from posts")

//1.3.0+
db.GetColumnValue<Post,string>(p=>p.Title,p=>p.Id==12)

//stored procedure support (1.2.0+) - see wiki for details
db.ExecuteStoredProcedure("spSomething",new{Param1="bla",_OutParam=0});

//quick helpers 1.3.0+
db.Count<Post>();
db.Count<Post>(p=>p.Title.StartsWith("Test"));

//check if table contains rows
db.HasAnyRows<Post>();
db.HasAnyRows<Post>(p=>p.IsActive);


var ids=new[]{1,2,3};
db.DeleteFrom<Post>(p=>ids.Contains(p.Id));

db.Drop<Post>();

if (db.TableExists<Post>()){ }

//insert 
var p= new Post{ Id=1, Title="Test"};
db.Insert(p);
p.Title="changed title";
db.Update<Post>(p);


//1.3.0+
// Update Post set Title=@0 where Id=23
db.Update<Post>(new{Title="bla"},p=>p.Id==23);

//update table sql builder

// update Post set [Count]=[Count]+1, IsActive=@0 where Id=12
db.Update<Post>().Set(p=>p.Count,p=>p.Count+1).Set(p=>p.IsActive,false).Where(p=>p.Id==12).Execute();


//paged queries , result contains Count and Items properties
var result=db.PagedQuery<Post>(0,5,"select * from post order by id desc");


//Multi poco mapping by convention similar to EF, no special setup

public class PostView
{
    public int Id {get;set}
    public string Title {get;set;}
    public IdName Author {get;set;} 
}

public class IdName
{
    public int Id {get;set;} // <- Author_Id
    public string Name {get;set;} // <- Author_Name
}

//'Author' is automatically instantiated and populated with data. The convention is to use [Property]_[Property]
var sql=@"
select p.Id, p.Title, p.AuthorId as Author_Id, u.Name as Author_Name 
from posts p inner join Users u on u.Id=p.AuthorId
where p.Id=@0";
var posts=db.Query<PostView>(sql,3);

//complex type mapping AND pagination with no special setup
result=db.PagedQuery<PostView>(0,10,sql,3)

````

#### Rules
* ALWAYS dispose the connection. If using a DI Container, ensure it calls dispose. Except when wanting to wrap a web request into a transaction, always set the DI Container to return a new instance.
* All the parameters in sql must be prefixed with '@' . The specific db provider will replace it with the proper prefix.
* _Enums_ are automatically handled from int or string when querying. When insert/update they are treated as ints. 
 * Use the [InsertAsStringAttribute] to save it as string
* Multi poco mapping is done automatically if a column name has '_'. That can be changed (1.2.0+)

```csharp
//let's use '.' instead of '_'
PocoFactory.ComplexTypeMapper.Separator = '.';
````
* If a table or column name is already escaped by the user, it won't be escaped by SqlFu
 * Every identifier containing a '.' will be split and each part will be escaped e.g dbo.table -> [dbo].[table]
* Dynamic results are read-only(2.0.0+)
* Any property/column which can't be mapped is ignored
 * However an exception is thrown if you want to assign a unsupported value to an object type for example, a null to a non-nullable

 
#### Attributes

``` csharp
[Table("Posts", PrimaryKey="Id" /*default*/,Autogenerated=true /*default*/,CreationOptions=IfTableExists.DropIt)]
public class Post
{
 public int Id {get;set;}
 [QueryOnly]
 public string ReadOnly {get;set;} //any property marked as QueryOnly will not be used for insert/update
 [InsertAsString]
 public MyEnum Type {get;set;}// any property with this attribute will be converted to string
}

```

## Versatility
* SqlFu knows how to map by default Nullable(T),Guid, TimeSpan, Enum and CultureInfo
* Allows manual mapping when you need it

``` csharp
//custom sql for those special cases
db.WithSql("select * from posts").Query<MyType>(reader=>{ /* mapping by hand */  })

db.WithSql("select item from myTable where id=@0",3).GetValue<myStruct>(result=> /* conversion by hand */)

//want to always use that mapper for every query
PocoFactory.RegisterMapperFor<MyType>(reader=> {/* do mapping */});
db.Query<MyType>(sql,args) // will automatically use the registered mapper instead of the default one

//same with converters
PocoFactory.RegisterConverterFor<myStruct>(obj=>{ /* do conversion */}) 

//or for value objects
PocoFactory.RegisterConverterFor<EmailValueObject>(obj=> new EmailValueObject(obj.ToString()))
db.GetValue<Email>("select email from users where id=@0",8)

//execute some command processing before query
db.WithSql(sql,args).Apply(cmd=> { /* modify DbCommand */}.Query<MyType>()

```
### Multi Poco mapping
As shown above, the only thing you need to do is to name the column according tot the [Property]_[Property] format. 
This feature is designed to be used for populating View Models where every object is a Poco with a parameterless constructor.

However if needed you can customize the instantion of any type (except the main Poco itself). Let's suppose you have this setup

``` csharp

public class Address
{
  //no parameterless constructor
  public Address(int userId)
  {
      UserId=userId;
  }
   
 public int UserId {get;private set;}
 public string Street {get;set;}
 public string Country {get;set;}
}

public class ViewModel
{
 /* ... other properties... */
 public Address Address {get;set;}
}

db.Query<ViewModel>("select u.* , addr.Street as Address_Street, addr.Country as Address_Country from users u, addresses addr where u.Id=4")

```

Ok, maybe the sql itself isn't very correct, that's not the point. The point is you want to populate that ViewModel and those are the relevant columns.
Since the Address object requires the userId, you can configure the **DefaultComplexTypeMapper** to use this for instantiating Address

``` csharp
  DefaultCompexTypeMapper.ToCreate<Address>(user=> new Address(user.Id));
  
```
The lambda is basically a Func<dynamic,T> and in this example the ViewModel is passed on as the dynamic argument.

What if you want to use your very own complex type mapping with any convention you like. It's a bit tricky but it's not hard.
You just need to implement the _IMapComplexType_ interface then assign it (you can also subclass the _DefaultComplexTypeMapper_ class).

```csharp
public class MyComplexMapper:IMapComplexType
{
 /* implementation */
}

PocoFactory.ComplexTypeMapper= new MyComplexMapper();

````

Note that the mapper should act as a singleton so it has to be thread safe. 
When implementing a complex mapper you have 2 ways to do it: in the _MapType<T>_ method or in the _EmitMapping_ method.
The first method is for normal people, the second involves emitting IL code with Reflection.Emit so it's aimed at the hardcore masochists. 
However, the second method is usually the most performant one. The SqlFu automapper will try to use the EmitMapping method first, but if it returns false, it will call the MapType method instead.

**Hint**: if you go for the first method and you want to set a property via reflection, use the _SetValueFast_ extension method (around 5-7x faster than reflection) defined in CavemanTools (used by SqlFu) and the same for the getters.

## Performance

Benchmarks for version 2.0.0 (.Net 4.0)

```
Executing scenario: FetchSingleEntity
-----------------------------------
SimpleData doesn't support the action. Specified method is not supported.
Massive doesn't support the action. not explicit type support
SqlFu FirstOrDefault - 500 iterations executed in 75,6658 ms
SqlFu Get - 500 iterations executed in 82,8085 ms
OrmLite - 500 iterations executed in 87,7121 ms
Dapper query entity - 500 iterations executed in 98,5202 ms
PetaPoco entity - 500 iterations executed in 102,525 ms
Dapper get entity - 500 iterations executed in 121,4952 ms
InsightDatabase - 500 iterations executed in 161,5671 ms

Executing scenario: FetchSingleDynamicEntity
-----------------------------------
SqlFu dynamic - 500 iterations executed in 79,0377 ms
OrmLite - 500 iterations executed in 81,8827 ms
InsightDatabase - 500 iterations executed in 83,8914 ms
PetaPoco dynamic - 500 iterations executed in 94,9423 ms
Dapper query entitty dynamic - 500 iterations executed in 96,2082 ms
Massive - 500 iterations executed in 129,1116 ms
SimpleData dynamic - 500 iterations executed in 133,3326 ms

Executing scenario: QueryTop10
-----------------------------------
SimpleData doesn't support the action. Specified method is not supported.
Massive doesn't support the action. not explicit type support
SqlFu - 500 iterations executed in 108,9925 ms
Dapper  - 500 iterations executed in 114,298 ms
PetaPoco - 500 iterations executed in 116,3961 ms
InsightDatabase - 500 iterations executed in 134,9926 ms
OrmLite - 500 iterations executed in 143,5561 ms

Executing scenario: QueryTop10Dynamic
-----------------------------------
OrmLite - 500 iterations executed in 114,2205 ms
SqlFu - 500 iterations executed in 117,3217 ms
Dapper  - 500 iterations executed in 126,3814 ms
InsightDatabase - 500 iterations executed in 141,544 ms
PetaPoco dynamic - 500 iterations executed in 143,2506 ms
Massive - 500 iterations executed in 327,3224 ms
SimpleData - 500 iterations executed in 405,4919 ms

Executing scenario: PagedQuery_Skip0_Take10
-----------------------------------
Dapper  doesn't support the action. No implicit pagination support
InsightDatabase doesn't support the action. No implicit pagination support
SqlFu - 500 iterations executed in 228,6695 ms
OrmLite - 500 iterations executed in 250,8005 ms
Massive - 500 iterations executed in 318,8973 ms
SimpleData - 500 iterations executed in 530,0301 ms
PetaPoco - 500 iterations executed in 1035,1179 ms

Executing scenario: ExecuteScalar
-----------------------------------
Dapper scalar - 500 iterations executed in 72,4849 ms
InsightDatabase - 500 iterations executed in 77,5271 ms
SqlFu scalar - 500 iterations executed in 82,1141 ms
SqlFu get scalar expression based - 500 iterations executed in 84,9293 ms
OrmLite - 500 iterations executed in 87,2419 ms
PetaPoco string - 500 iterations executed in 87,8697 ms
Massive - 500 iterations executed in 124,431 ms
SimpleData scalar - 500 iterations executed in 154,2036 ms

Executing scenario: MultiPocoMapping
-----------------------------------
SimpleData doesn't support the action. Specified method is not supported.
Massive doesn't support the action. Specified method is not supported.
OrmLite doesn't support the action. Suports only its own specific source format
SqlFu - 500 iterations executed in 88,1978 ms
Dapper  - 500 iterations executed in 88,6813 ms
PetaPoco - 500 iterations executed in 97,1947 ms
InsightDatabase - 500 iterations executed in 97,8031 ms

Executing scenario: Inserts
-----------------------------------
InsightDatabase doesn't support the action. Specified method is not supported.
massive doesn't support the action. Couldn't figure how to insert pocos with auto increment id
SqlFu - 500 iterations executed in 138,7476 ms
PetaPoco - 500 iterations executed in 153,3651 ms
Dapper - 500 iterations executed in 169,0831 ms
OrmLite - 500 iterations executed in 188,4134 ms
SimpleData - 500 iterations executed in 362,3814 ms

Executing scenario: Updates
-----------------------------------
InsightDatabase doesn't support the action. Specified method is not supported.
OrmLite - 500 iterations executed in 104,1065 ms
PetaPoco - 500 iterations executed in 107,2367 ms
SqlFu - 500 iterations executed in 108,4179 ms
Dapper  - 500 iterations executed in 188,0426 ms
massive - 500 iterations executed in 238,6045 ms
SimpleData - 500 iterations executed in 263,6972 ms
 

````

