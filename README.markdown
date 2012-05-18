#Welcome to SqlFu

SqlFu is a  dapper fast micro-orm (like dapper.net, peta poco , massive etc) for .Net 4.  

## Why should you use it
The main USP (unique selling proposition - advantage) of SqlFu  is  **Versatility**. This is the reason I've developed it. I need it more flexibility and the micro-orm I was using (peta poco) didn't have it and if other micro-orms had it, they were too slow (FluentData). 

I've designed SqlFu based on three equally important principles:
 User Friendliness (intuitivity) | Versatility |  Performance
 
 **What SqlFu is**:  Fast library built on top of Ado.Net to help you reduce the amount of coding by hand. You still have to write Sql, but you won't have to deal with repetitive chores.
 
 **What SqlFu is NOT**: A replacement for any ORM abstracting sql or generating DDL for you. 
  
### User Frendly
 
 
 ```csharp
 
var db= new DbAccess(connection,DbType.SqlServer);

//usual stuff
db.Query<Post>("select * from posts where id=@0",1);

//you can pass ordinal params or anonymous objects
db.Query<Post>("select * from posts where id=@id",new{id=1});

db.ExecuteScalar<int>("select count(*) from posts")

//insert 
var p= new Post{ Id=1, Title="Test"};
db.Insert(p);
p.Title="changed title";
db.Update<Post>(p);

//paged queries , result contains Count and Items properties
var result=db.PagedQuery<Post>(0,5,"select * from post order by id desc");

//complex type mapping similar to EF

public class IdName
{
    public int Id {get;set;}
    public string Name {get;set;}    
}

public class PostView
{
    public int Id {get;set}
    public string Title {get;set;}
    public IdName Author {get;set;}
}

//Author is automatically instantiated and populated with data. The convention is to use [Property]_[Property]
var posts=db.Query<PostView>(@"
select p.Id, p.Title, p.AuthorId as Author_Id, u.Name as Author_Name 
from posts p inner join Users u on u.Id=p.AuthorId
where p.Id=@0",3)

 ````

#### Traits
* All the parameters in sql must be prefixed with '@' . The specific db provider will replace it with the proper prefix.
* _Enums_ are automatically handled from int or string when querying. When insert/update they are treated as ints.
* Complex type mapping is done automatically if a column name has '_'.
* Any property/column which can't be mapped is ignored
* However an exception is thrown if you want to assign a value to an object type for example, or null to a non-nullable

 
### Versatility
``` csharp

//custom sql for those special cases
db.WithSql("select * from posts").ExecuteQuery<Post>(reader=>{ /* mapping by hand */  })

db.WithSql("select item from myTable where id=@0",3).ExecuteScalar<myStruct>(result=> new myStruct(result.ConvertTo<string>()))

//want to always use that mapper for every query
PocoFactory.RegisterMapperFor<MyType>(reader=> {/* do mapping */});
db.Query<MyType>(sql,args) // will automatically use the registered mapper instead of the default one

//same with converters
PocoFactory.RegisterConverterFor<myStruct>(obj=>{ /* do conversion */}) 

//or for value objects
PocoFactory.RegisterConverterFor<EmailValueObject>(obj=> new EmailValueObject(obj.ToString()))

db.ExecuteScalar<Email>("select email from users where id=@0",8)
```

