using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
namespace SqlFu
{

  public class SqlFuDynamic:DynamicObject
  {
      private KeyValuePair<string, object>[] _sqlData;

      public SqlFuDynamic(KeyValuePair<string,object>[] sqlData)
      {
            _sqlData = sqlData;
      }

      public override bool TryGetMember(GetMemberBinder binder, out object result)
      {
          var item = _sqlData.FirstOrDefault(d => d.Key.Equals(binder.Name, StringComparison.InvariantCultureIgnoreCase));
          result = item.Value;
          return item.Key != null;
      }

      public override bool TrySetMember(SetMemberBinder binder, object value)
      {
          throw new InvalidOperationException("This object is read only");          
      }

  }

       
    }

