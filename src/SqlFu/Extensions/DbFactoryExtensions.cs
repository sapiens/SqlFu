using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlFu
{
    public static class DbFactoryExtensions
    {
        public static async Task<T> DoAsync<T>(this IDbFactory fac, Func<DbConnection,Task<T>> act,CancellationToken? cancel=null)
        {
            
            using (var db = await fac.CreateAsync(cancel??CancellationToken.None).ConfigureFalse())
            {
                return await act(db).ConfigureFalse();
            }
        }
        
        
        public static T Do<T>(this IDbFactory fac, Func<DbConnection,T> act)
        {
            
            using (var db = fac.Create())
            {
                return act(db);
            }
        }
    }
}