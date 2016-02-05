using SqlFu.Providers;

namespace Tests.Builders
{
    public class TestDbProviderExpression:AbstractProviderExpressions
    {
         public static IDbProviderExpressions Instance=new TestDbProviderExpression();
    }
}