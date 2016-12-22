using SqlFu.Providers;

namespace Tests._Fakes
{
    public class TestDbProviderExpression:DbProviderExpressions
    {
       public static TestDbProviderExpression Instance=new TestDbProviderExpression();
    }
}