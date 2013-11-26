#Welcome to SqlFu

SqlFu is a **_versatile_** micro-orm (like dapper.net, peta poco , massive etc) for .Net 4.  SqlFu is Apache licensed.
If you're wondering if there's a reason for yet another micro-orm [read this](http://www.sapiensworks.com/blog/post/2012/05/19/SqlFu-My-Versatile-Micro-Orm.aspx)

Latest version: 2.1.0 [Change Log](https://github.com/sapiens/SqlFu/wiki/ChangeLog)

**Version 2 is not compatbile with version 1**. [Read how to upgrade to SqlFu 2](https://github.com/sapiens/SqlFu/wiki/How-To-Upgrade-to-SqlFu2)

SqlFu uses Apache 2.0 license.


### Note for contributors

Please create your pull requests to target the "devel" branch. "Master" is only for released code. Thank you.

## Why should you use it
The main USP (unique selling proposition - advantage) of SqlFu  is  **Versatility**. This is the reason I've developed it. I need it more flexibility and the micro-orm I was using (peta poco) didn't have it and if other micro-orms had it, they were too slow. 

I've designed SqlFu based on three equally important principles:
 
 **User Friendliness** - **Versatility** -  **Performance**
 
SqlFu supports
* SqlServer 2005+
* MySql
* Postgresql
* SqlServerCE 4 (new in SqlFu 1.1)
* Sqlite (new in SqlFu 1.1)

Read about the **[Advanced Features](https://github.com/sapiens/SqlFu/wiki)**

## User Friendly
 
 Intuitive usage and automatic multi poco mapping by convention, similar to EF. Multi poco mapping automatically works with pagination without any special setup.

##Usage

### Config
```csharp

//convenience 
SqlFuDao.ConnectionStringIs("connection string",DbEngine.SqlServer);
SqlFuDao.ConnectionNameIs("connection string name") //as configured in app(web).config

//setup logging
SqlFuDao.OnCommand = cmd => Console.WriteLine(cmd.FormatCommand());
SqlFuDao.OnException = (cmd,ex)=>Console.WriteLine("\nSql:{1}\nException:\n\t\t{0}", ex,cmd.FormatCommand());

````


### Get Connection

```csharp
//if no config is set as above, it will try to use the first connection found in app(web).config
using (var db=SqlFuDao.GetConnection()) 
{
  //db stuff
}

//or

using(DbConnection db=new SqlFuConnection(connection,DbType.SqlServer))
{
 //db stuff
}

````


#### v1 (legacy)
```csharp
var db= new DbAccess(connection,DbType.SqlServer);
````

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

