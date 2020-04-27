using SfRedis.Sessions.Redis;
using StackExchange.Redis;
using System;

namespace SfRedis.Sessions
{
    class RedisSession : ImplSession
    {
        String _Name="localhost";
        
        String _Host="localhost";
        
        String _Port="6379";
        
        String _DB;

        String _Password;

        IDatabase ctx;

        public string Port { get => _Port; set => _Port = value; }
        public string Host { get => _Host; set => _Host = value; }

        public string Name { get => _Name; set => _Name = value; }
        public string DB { get => _DB; set => _DB = value; }
        public string Password { get => _Password; set => _Password = value; }


        new public void Connect() {
            var  redis = ConnectionMultiplexer.Connect(Host);
            ctx = redis.GetDatabase();
            System.Windows.MessageBox.Show("建立连接");
        }

        new public void Create()
        {
            RedisForm redisForm = new RedisForm();
            redisForm.Show();
        }

    }
}
