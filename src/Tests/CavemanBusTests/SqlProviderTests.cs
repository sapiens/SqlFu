using System.Linq;
using CavemanTools.Infrastructure.MessagesBus;
using CavemanTools.Logging;
using SqlFu;
using SqlFu.CavemanBus;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.CavemanBusTests
{
    public class MyCommand:AbstractCommand
    {
        public ITest Test { get; set; }
    }

    public interface ITest
    {
        int Id { get; }
    }

    class MyTest:ITest
    {
        public int Id { get; set; }
    }

    public class SqlProviderTests
    {
        private Stopwatch _t = new Stopwatch();
        private MessageBusSqlStorage _store;

        public SqlProviderTests()
        {
           LogHelper.Remove("dev");
            LogHelper.Register(new ConsoleLogger(), "dev");
            _store = new MessageBusSqlStorage(Config.Connex,DBType.SqlServer);
        }

        [Fact]
        public void init_storage()
        {
            _store.DestroyStorage();
            Assert.False(_store.IsStoreCreated());
            _store.EnsureStorage();
            Assert.True(_store.IsStoreCreated());
        }

        [Fact]
        public void insert_message()
        {
            _store.EmptyStore();
            var cmd = new MyCommand()
                          {
                              Test = new MyTest()
                                         {
                                             Id = 34
                                         }
                          };
            _store.StoreMessageInProgress(cmd);
            var all = _store.GetUncompletedMessages().ToArray();
            Assert.Equal(1,all.Count());
        }

        [Fact]
        public void complete_message()
        {
            _store.EmptyStore();
            var cmd = new MyCommand()
            {
                Test = new MyTest()
                {
                    Id = 34
                }
            };
            
            _store.StoreMessageInProgress(cmd);
            _store.StoreMessageCompleted(cmd.Id);

            cmd = new MyCommand()
            {
                Test = new MyTest()
                {
                    Id = 45
                }
            };

            _store.StoreMessageInProgress(cmd);
            _store.StoreMessageCompleted(cmd.Id);
            _store.StoreMessageCompleted(cmd.Id);

            var all = _store.GetUncompletedMessages().ToArray();
            Assert.Equal(0, all.Count());
        }

        [Fact]
        public void duplicate_message_throws()
        {
            _store.EmptyStore();
            var cmd = new MyCommand()
            {
                Test = new MyTest()
                {
                    Id = 34
                }
            };
            _store.StoreMessageInProgress(cmd);
            Assert.Throws<DuplicateMessageException>(() =>
                                                         {
                                                             _store.StoreMessageInProgress(cmd);
                                                         });
            
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }


    }
}