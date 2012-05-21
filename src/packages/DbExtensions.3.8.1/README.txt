DbExtensions
=============================================================================== 

DbExtensions consists of basically 3 components that can be used together 
or separately: 1- A set of extension methods that simplifies raw ADO.NET 
programming, 2- A dynamic SQL API, 3- A CRUD API that integrates with 
System.Data.Linq.Mapping.

Licensing
=============================================================================== 

This software is licensed under the terms you may find in the file
named "LICENSE.txt" of this distribution.

Getting Started
=============================================================================== 

Please refer to the website for information on how to get started
  
  http://dbextensions.sf.net/

Please help us make DbExtensions better - we appreciate any feedback
you may have.

$ Donate
=============================================================================== 

If you would like to show your appreciation for DbExtensions, please 
consider making a small donation

  http://dbextensions.sf.net/donate/

Changes
=============================================================================== 
3.8.1 - Added DataAccessObject.Count<T>(string, object[]) and 
        DataAccessObject.Count(Type, string, object[]) methods.
      - Added DataAccessObject.LongCount<T>(string, object[]) and 
        DataAccessObject.LongCount(Type, string, object[]) methods.
      - Added DataAccessObject.DeleteById method.
      - Added DataAccessObject.DELETE_FROM_WHERE(Type, object) overload.
      - Added DbProviderFactory.CreateCommand(SqlBuilder) and 
        DbConnection.CreateCommand(SqlBuilder) extension methods.

3.8.0 - Added DbConnection.Count() and DbConnection.LongCount() overloads 
        that take a TextWriter for logging.
      - Added DbConnection.Exists(SqlBuilder) method.
      - Added SqlBuilder.Insert(int, string) method for inserting a string 
        at the specified index.
      - Added SqlBuilder.JoinSql method.
      - Added DataAccessObject(MetaModel) constructor.
      - Added DataAccessObject.INSERT_INTO_VALUES method.
      - Added DataAccessObject.DELETE_FROM method.
      - Added DataAccessObject.DELETE_FROM_WHERE method.
      - Added DataAccessObject.UPDATE_SET_WHERE method.
      - Added DataAccessObject.InsertDeep method.
      - Added DataAccessObject.InsertMany method.
      - Added DataAccessObject.Exists(SqlBuilder), 
        DataAccessObject.Exists&lt;T>(string, object[]) and 
        DataAccessObject.Exists(Type, string, object[]) methods.
      - Added System.Transactions integration in 
        DataAccessObject.EnsureInTransaction .
      - Renamed DataAccessObject.GetLastInsertId to LastInsertId.
      - Removed obsolete members.
      - Complete XML documentation comments.

3.7.1 - Fixed 2 bugs.