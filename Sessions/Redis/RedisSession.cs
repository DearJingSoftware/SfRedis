using SfRedis.Sessions.Redis;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
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
        //,INotifyPropertyChanged
    {

        Boolean _IsConnected = false;

        String _Name = "localhost";

        String _Host = "localhost";

        String _Port = "6379";

        String _DB="0";

        String _Password;

        Dictionary<String, Object> ctx = new Dictionary<string, object>();

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
        private IServer _Server;

        //public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SfRedisKey> Keys { get => _Keys; set => _Keys = value; }
        public ConnectionMultiplexer Conn { get => conn; set => conn = value; }
        public IServer Server { get => _Server; set => _Server = value; }
        public bool IsConnected { get => _IsConnected; set => _IsConnected = value; }

        new public void Connect()
        {
       
            MainWindow.ctxSession = this;
            Conn = ConnectionMultiplexer.Connect(Host + ":" + Port);
            IsConnected = Conn.IsConnected;
            Server = conn.GetServer(Host, int.Parse(Port));
              
            
            Keys.Clear();
            foreach (var entry in Server.Keys(pattern:"*"))
            {
                Keys.Add(new SfRedisKey { Name = entry, Session=this });
            }


            //当前执行结果
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
            string[] args = text.Split(' ');
            string[] arg1 = SubArray(args, 1, args.Length - 1);
            ctxResult = Server.Execute(args[0], arg1);
        }


        override  public string GetIdentifier()
        {
            return Name;
        }

        override public void Refresh(Session session)
        {
            
            Keys.Clear();
            foreach (var entry in Server.Keys(pattern: "*"))
            {
                Keys.Add(new SfRedisKey { Name = entry, Session = this });
            }
        }

        override public void DisConnect()
        {
            Conn.Close();
            Keys.Clear();
            IsConnected = Conn.IsConnected;
            MessageBox.Show(conn.IsConnected.ToString());
        }

        override public void ReConnect()
        {
            if (Conn.IsConnected) {
                Conn.Close();
            }
            Connect();
        }


    }
}
