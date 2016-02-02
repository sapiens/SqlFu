using System;
using System.Linq;
using System.Threading.Tasks;
using CavemanTools.Logging;
using Xunit.Abstractions;

namespace DomainBus.Tests
{
    public static class AssertionsExtensions
    {
        public static Func<Task> InvokingAsync<T>(this T sut, Func<T,Task> method)
        {
            return ()=>method(sut);
        }

        public static TException ShouldThrow<TException>(this Func<Task> task) where TException : Exception
        {
        //    var result = false;
            try
            {
                task().Wait();
            }
            catch (AggregateException ex)
            {
                var inner= ex.InnerExceptions.OfType<TException>().FirstOrDefault();
                if (inner == null) throw;
                return inner;
            }
            return null;
        }

        public static void Logger(this ITestOutputHelper x)
        {
            LogManager.OutputTo(x.WriteLine);
        }
    }
}