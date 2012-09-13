using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using CavemanTools.Infrastructure.MessagesBus;
using CavemanTools.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace SqlFu.CavemanBus
{

    public class MessageCommit
    {
        public MessageCommit()
        {

        }

        JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings()
                       {
                           TypeNameHandling =
                               TypeNameHandling.Objects,
                           PreserveReferencesHandling =
                               PreserveReferencesHandling.
                               Arrays
                       };
        }

        public MessageCommit(IMessage msg)
        {
            Body = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg,GetJsonSettings() ));

            UniqueConstraint = GenerateKey(msg);
            MessageId = msg.Id;
            CommittedAt = DateTime.UtcNow;
            // var serializer = new JsonSerializer();
            // serializer.PreserveReferencesHandling=PreserveReferencesHandling.Arrays;
            //using(var ms= new MemoryStream())
            //{
            //    using(var writer=new BsonWriter(ms))
            //    {
            //        serializer.
            //    }
            //}
        }

        string GenerateKey(IMessage msg)
        {
            var sb = new StringBuilder();
            sb.Append(msg.SourceId.Value);
            msg.ToDictionary().Where(d => d.Key != "Id" && d.Key != "SourceId" && d.Key != "TimeStamp").Select(kv => JsonConvert.SerializeObject(kv.Value)).ForEach(s => sb.Append(s));
            return sb.ToString().Sha1();
        }

        public long Id { get; set; }
        public Guid MessageId { get; set; }
        public DateTime CommittedAt { get; set; }
        public byte[] Body { get; set; }
        public int State { get; set; }
        public string UniqueConstraint { get; set; }

        public IMessage ToMessage()
        {
            var js = Encoding.Unicode.GetString(Body);
            var t = JsonConvert.DeserializeObject(js,GetJsonSettings());
            return (IMessage) t;
        }
    }

    internal class MySqlProvider:IMessageBusStoreProvider
    {
        private readonly DbAccess _db;
        private const string CreateStoreSql = @"CREATE TABLE `messagebusstore` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `MessageId` char(36) NOT NULL,
  `CommittedAt` datetime DEFAULT NULL,
  `Body` varbinary(255) DEFAULT NULL,
  `State` int(11) NOT NULL,
  `UniqueConstraint` varchar(80) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uc_message` (`UniqueConstraint`),
  KEY `ix_message_id` (`MessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

        public MySqlProvider(DbAccess db)
        {
            _db = db;
        }

        public bool IsStorageInitiated()
        {
            return
                _db.ExecuteScalar<int>(
                    string.Format(@"SELECT COUNT(*)
FROM information_schema.tables 
WHERE table_schema = '{0}' 
AND table_name = '{1}'",_db.Connection.Database,MessageBusSqlStorage.TableName))>0;
        }

        public void CreateStore()
        {
            _db.ExecuteCommand(CreateStoreSql);
        }

        public bool IsUniqueKeyViolation(DbException ex)
        {
            return ex.Message.Contains("uc_message");
        }
    }

    internal class SqlServerStrings:IMessageBusStoreProvider
    {
        private readonly DbAccess _db;

        public const string CreateStoreSql =
            @"CREATE TABLE [dbo].[MessageBusStore] (
[Id] bigint NOT NULL IDENTITY(1,1) ,
[MessageId] uniqueidentifier NOT NULL ,
[CommittedAt] datetime NULL ,
[Body] varbinary(MAX) NULL ,
[State] int NOT NULL ,
[UniqueConstraint] varchar(80) COLLATE Latin1_General_CI_AI NULL ,
CONSTRAINT [PK__MessageB__3214EC0703317E3D] PRIMARY KEY ([Id]),
CONSTRAINT [uc_message] UNIQUE ([UniqueConstraint] ASC)
)";

        public SqlServerStrings(DbAccess db)
        {
            _db = db;
        }

        public bool IsStorageInitiated()
        {
            return _db.ExecuteScalar<int>(string.Format(
                @"SELECT count(*)
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '{0}'",MessageBusSqlStorage.TableName)) >0;
        }

        public void CreateStore()
        {
            _db.ExecuteCommand(CreateStoreSql);
        }

        public bool IsUniqueKeyViolation(DbException ex)
        {
            return ex.Message.Contains("uc_message");
        }
    }


    interface IMessageBusStoreProvider
    {
        bool IsStorageInitiated();
        void CreateStore();
        bool IsUniqueKeyViolation(DbException ex);
    }

    public class MessageBusSqlStorage:IStoreMessageBusState
    {
        private DbAccess _db;

        public const string TableName = "MessageBusStore";

        public MessageBusSqlStorage(string connectionName)
        {
            _db = new DbAccess(connectionName);
            InitProvider();
        }

        public MessageBusSqlStorage(string cnxString,DbEngine provider)
        {
            _db= new DbAccess(cnxString,provider);
            InitProvider();
        }

        private IMessageBusStoreProvider _provider;

        void InitProvider()
        {
            switch(_db.Provider.ProviderType)
            {
                case DbEngine.SqlServer: _provider = new SqlServerStrings(_db);
                    break;
                case DbEngine.MySql: _provider = new MySqlProvider(_db);
                    break;
                default:throw new NotImplementedException("There is no provider available for this database engine");
            }
            LogHelper.DefaultLogger.Debug("[MessageBusStore] Using provider for "+_db.Provider.ProviderType);
        }

        public bool IsStoreCreated()
        {
            return _provider.IsStorageInitiated();
        }

        public void EnsureStorage()
        {
           LogHelper.DefaultLogger.Debug("[MessageBusStore] Checking if storage is initiated...");
            if (!_provider.IsStorageInitiated())
           {
               LogHelper.DefaultLogger.Debug("[MessageBusStore] Initiating storage...");
                _provider.CreateStore();
                LogHelper.DefaultLogger.Debug("[MessageBusStore] Storage created");
           }
        }

        /// <summary>
        /// Deletes underlying table
        /// </summary>
        public void DestroyStorage()
        {
            _db.ExecuteCommand("drop table "+ TableName);
        }

        public void StoreMessageInProgress(IMessage cmd)
        {
            var commit = new MessageCommit(cmd);
            commit.State = 1;
            try
            {
                _db.Insert(TableName,commit);
            }
            catch(DbException ex)
            {
                if (_provider.IsUniqueKeyViolation(ex))
                {
                    throw new DuplicateMessageException();
                }
                else
                {
                    throw;
                }
            }
        }

        public void StoreMessageCompleted(Guid id)
        {
            _db.Insert(TableName, new{MessageId=id,State=-1, CommittedAt=DateTime.UtcNow,UniqueConstraint=id});
            //maybe catch unique key violation?!
        }

        public IEnumerable<IMessage> GetUncompletedMessages()
        {
            return _db.Fetch<MessageCommit>("select Body from " + TableName + " where Messageid in (select MessageId from " + TableName +
                                     " group by MessageId having sum(State)=1)").Select(m=>m.ToMessage());
        }

        public void EmptyStore()
        {
            _db.ExecuteCommand("truncate table " + TableName);
        }

        public void Cleanup()
        {
            _db.ExecuteCommand("delete from " + TableName + " where MessageId in (select MessageId from " + TableName +
                               " group by MessageId having sum(State)=0)");
        }
    }
}