using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model;
using SqlFu.Mapping.Internals;
using Xunit;

namespace SqlFu.Tests
{
    


    public class General
    {
        public General()
        {
            //post id

        }

        [Fact]
        public void encode()
        {
            var salt = Enumerable.Repeat(' ',16).ToArray();

            var encr = "Xemerys really smashed ballZ".EncryptAsString(new string(salt));
            Console.WriteLine(encr);
            Console.WriteLine(encr.DecryptAsString(new string(salt)));
        }
       
        [Fact]
        public async Task testName()
        {
            
           // SqlFuManager.Config.ConnectionString = Setup.Connex;
            
            using (var cnx=SqlFuManager.OpenConnection(SqlFuManager.DefaultProvider,Setup.Connex))
            {
               Setup.DoBenchmark(50, p =>
               {
                   var t = Task.Run(async () =>
                   {
                       var cmd = cnx.CreateAndSetupCommand("select 1 as Id, 'bula' as Name");
                       await cmd.FetchAsync<IdName>(CancellationToken.None);
                   });
                   t.Wait();

               },
                privprov =>
                {
                    var t=

                    Task.Run(async ()=>{
                        using (var cmd = cnx.CreateCommand())
                        {
                            cmd.CommandText = "select 1 as Id, 'bula' as Name";
                            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                            {
                                while (await reader.ReadAsync().ConfigureAwait(false))
                                {
                                    //dynamic d=new ExpandoObject();
                                    var d=new IdName();
                                    d.Id = (int)reader["Id"];
                                    d.Name = reader["Name"] as string;
                                }
                            }
                        } 
                    });
                   t.Wait();


                });

              
            }
            }
        }

    
}