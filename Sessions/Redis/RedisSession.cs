using SfRedis.Sessions.Redis;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows.Input;

namespace SfRedis.Sessions
{
    class RedisSession : ImplSession
    {
        String _Name="localhost";
        
        String _Host="localhost";
        
        String _Port="6379";
        
        String _DB;

        String _Password;

        Dictionary<String,Object> ctx=new Dictionary<string, object>();

        IDatabase db;

        List<String> _Keys=new List<string>();

        ResultType ctxType;

        RedisResult ctxResult;

        public string Port { get => _Port; set => _Port = value; }
        public string Host { get => _Host; set => _Host = value; }

        public string Name { get => _Name; set => _Name = value; }
        public string DB { get => _DB; set => _DB = value; }
        public string Password { get => _Password; set => _Password = value; }
        public List<string> Keys { get => _Keys; set => _Keys = value; }

        new public void Connect() {
            MainWindow.ctxSession = this;
            var  redis = ConnectionMultiplexer.Connect(Host);
            //当前db
            db = redis.GetDatabase();
            string[] argv = { "*" };
            var res = db.Execute("keys", argv);
            foreach (KeyValuePair<string, RedisResult> entry in res.ToDictionary())
            {
                Keys.Add(entry.Value.ToString());
            }

            //当前执行结果
            ctx["db"]= db;
            ctx["ctxResult"]=ctxResult;
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

        new public void Command(string text)
        {
            string[] args = text.Split(' ');
            string[] arg1 = SubArray(args, 1, args.Length - 1);
            ctxResult = db.Execute(args[0], arg1);
        }


    }
}
