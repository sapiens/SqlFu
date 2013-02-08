using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Benchmark.Tests;
using CavemanTools.Testing;
using Tests;

namespace Benchmark
{
    class Program
    {
        private static Dictionary<string, object> Params= new Dictionary<string, object>();
        static void Main(string[] args)
        {
            Config.EnsureDb();             
            var p = new Program();
            try
            {
                p.RunAll();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        private PerformanceTests[] _classes;
        private MethodInfo[] _methods;

         List<BenchmarksContainer> _bcs = new List<BenchmarksContainer>();            

        public Program()
        {
            _classes = Assembly.GetExecutingAssembly().GetTypesImplementing<PerformanceTests>().Where(
                tp => tp.Name != "PerformanceTests").Select(tp=>Activator.CreateInstance(tp)).Cast<PerformanceTests>().ToArray();
            _methods = typeof (PerformanceTests).GetMethods().Where(mi=>mi.IsAbstract).ToArray();

            foreach(var mi in _methods)
            {
                
                var bc = new BenchmarksContainer(mi.Name);
                _bcs.Add(bc);
                foreach(var cls in _classes)
                {
                    mi.Invoke(cls, new object[] {bc});
                }
            }
        }


        public void RunAll()
        {
            using (var f = File.CreateText("results.txt"))
            {
                
                foreach (var mi in _methods)
                {
                    Config.EnsurePosts();
                    Console.WriteLine("\nExecuting scenario: " + mi.Name + "\n-----------------------------------");
                    f.WriteLine("\nExecuting scenario: " + mi.Name + "\n-----------------------------------");
                    var bc = _bcs.First(d => d.ContainerName == mi.Name);
                    bc.WarmUpIterations = 5;
                    bc.ExecuteWarmup();
                    bc.Execute();

                    foreach (var rez in bc.GetResults.OrderBy(r => r.Total))
                    {
                        Console.WriteLine(rez.ToString(false));
                        f.WriteLine(rez.ToString(false));
                    }
                    f.Flush();
                }
               
            }
        }

    }
}
