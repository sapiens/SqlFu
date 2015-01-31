DbExtensions
=============================================================================== 
Extension methods for ADO.NET, CRUD and dynamic SQL components.

Licensing
=============================================================================== 

This software is licensed under the terms you may find in the file
named "LICENSE.txt" of this distribution.

Getting Started
=============================================================================== 

Please refer to the website for information on how to get started
  
  https://github.com/maxtoroq/DbExtensions

Please help us make DbExtensions better - we appreciate any feedback
you may have.

$ Donate
=============================================================================== 

If you would like to show your appreciation for DbExtensions, please 
consider making a small donation

  https://github.com/maxtoroq/DbExtensions/wiki/Donate

Changes
=============================================================================== 
DbExtensions releases follow Semantic Versioning rules http://semver.org

4.2.0 - Mapping to dynamic objects, Map methods which do not need a Type argument,
        also supported by SqlSet. This feature requires .NET 4+
      - New methods: SqlTable.ContainsKey, SqlCommandBuilder.UPDATE, 
        SqlTable.DeleteRange, SqlTable.UpdateRange
      - Deprecated SqlTable.InsertDeep, replaced by Insert(object, bool)
      - Added SqlTable.InsertRange overloads with bool parameter
      - Deprecated Database.InsertRange
      - Deprecated SqlTable.DeleteById, replaced by DeleteKey

4.1.1 - Fixed #4: SqlSet<T>.AsEnumerable() fails if T is a value type
      - Fixed #2: Use parameter on SqlBuilder OFFSET(int) and LIMIT(int)
      - Fixed #3: OleDbConnection and OdbcConnection do not override DbProviderFactory

4.1.0 - Mapping to constructor arguments
      - Copied DbFactory methods to Database, marked DbFactory as obsolete
      - Fixed #1: Skip Refresh after Insert on non-entity types

4.0.0 - New SqlSet API for making queries
      - DataAccessObject split into Database, DatabaseConfiguration and SqlTable
      - IDataRecord.Get{TypeName}(string) extension methods
      - MapXml methods now return XmlReader, and take XmlMappingSettings parameter
      - Unified extension methods class
