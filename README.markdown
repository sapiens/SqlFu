#Welcome to SqlFu

SqlFu is a  dapper fast micro-orm (like dapper.net, peta poco , massive etc) for .Net 4.  

## Why should you use it
The main USP (unique selling proposition - advantage) of SqlFu  is  **Versatility**. This is the reason I've developed it. I need it more flexibility and the micro-orm I was using (peta poco) didn't have it and if other micro-orms had it, they were too slow (FluentData). 

I've designed SqlFu based on three equally important principles:
 User Friendliness (intuitivity) | Versatility |  Performance
 
 **What SqlFu is**:  Fast library built on top of Ado.Net to help you reduce the amount of coding by hand. You still have to write Sql, but you won't have to deal with repetitive chores.
 
 **What SqlFu is NOT**: A replacement for any ORM abstracting sql or generating DDL for you. 
 
  ## User Frendly
 
 ```csharp
 
var db= new DbAccess(connection,DbType.SqlServer);

//usual stuff
db.Query<Post>("select * from posts where id=@0",1);

//you can pass ordinal params or anoonymous objects
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



 ````
  
All the parameters in sql msut be prefixed with '@' . The specific db provider will replace it with the proper prefix.
 

 
 

