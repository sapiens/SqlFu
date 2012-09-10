using Xunit;
using System;
using System.Diagnostics;

namespace Tests
{
    public class ComplexMapperTests
    {
        private Stopwatch _t = new Stopwatch();

        public ComplexMapperTests()
        {

        }

        [Fact]
        public void test()
        {
            
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}