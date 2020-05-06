using SfRedis.Sessions.Redis;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;

namespace SfRedis.Sessions
{
    class SfRedisKey
    {
        String _Name = "";

        Session _Session;

        public string Name { get => _Name; set => _Name = value; }
        public Session Session { get => _Session; set => _Session = value; }
    }
    class RedisSession : ImplSession
    {
        String _Name = "localhost";

        String _Host = "localhost";

        String _Port = "6379";

        String _DB;

        String _Password;

        Dictionary<String, Object> ctx = new Dictionary<string, object>();

        IDatabase db;

        ObservableCollection<SfRedisKey> _Keys = new ObservableCollection<SfRedisKey>();

        ResultType ctxType;

        RedisResult ctxResult;
        public Session _Session;
        public string Port { get => _Port; set => _Port = value; }
        public string Host { get => _Host; set => _Host = value; }

        public string Name { get => _Name; set => _Name = value; }
        public string DB { get => _DB; set => _DB = value; }
        public string Password { get => _Password; set => _Password = value; }

        private ConnectionMultiplexer conn;
        public ObservableCollection<SfRedisKey> Keys { get => _Keys; set => _Keys = value; }
        public ConnectionMultiplexer Conn { get => conn; set => conn = value; }

        new public void Connect()
        {
            MainWindow.ctxSession = this;
            conn = ConnectionMultiplexer.Connect(Host);
            //当前db
            db = conn.GetDatabase();
            string[] argv = { "*" };
            RedisResult res = db.Execute("keys", argv);


            var server = conn.GetServer("localhost:6379");
            Keys.Clear();
            foreach (var entry in server.Keys(pattern:"*"))
            {
                Keys.Add(new SfRedisKey { Name = entry, Session=this });
            }

            //当前执行结果
            ctx["db"] = db;
            ctx["ctxResult"] = ctxResult;
        }

        new public void Create()
        {
            RedisForm redisForm = new RedisForm();
            redisForm.Show();
        }

        public static String[] SubArray<String>(String[] data, int index, int length)
        {
            String[] result = new String[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        override public void Command(string text)
        {
            MessageBox.Show(text);
            string[] args = text.Split(' ');
            string[] arg1 = SubArray(args, 1, args.Length - 1);
            ctxResult = db.Execute(args[0], arg1);
        }


        override  public string GetIdentifier()
        {
            return Name;
        }

        override public void Refresh(Session session)
        {
            var server = conn.GetServer("localhost:6379");
            Keys.Clear();
            foreach (var entry in server.Keys(pattern: "*"))
            {
                Keys.Add(new SfRedisKey { Name = entry, Session = this });
            }
        }


    }
}
