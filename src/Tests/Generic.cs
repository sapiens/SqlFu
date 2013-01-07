using SqlFu;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests
{
    public class Generic
    {
        private Stopwatch _t = new Stopwatch();

        public Generic()
        {

        }

       // [Fact]
        public void stored()
        {
            var st = Setup.GetDb().ExecuteStoredProcedure("bla",new{Param1="bla",_Param2=0});
            Assert.Equal(1,st.ReturnValue);
            Assert.Equal(34,st.OutputValues["Param2"]);
        }

        public enum TT:long
        {
            None=0
        }

        [Fact]
        public void FactMethodName()
        {
           

        }


        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}