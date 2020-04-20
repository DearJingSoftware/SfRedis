using Microsoft.Win32;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SfRedis
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
      
        class TextWriter1 : TextWriter
        {
            internal TextBox textBlock;

            public override Encoding Encoding => throw new NotImplementedException();
            public override void WriteLine(string value)
            {
                //写入session 日志
                base.Write(value);
                textBlock.Dispatcher.BeginInvoke(new Action(delegate
                {
                    textBlock.Text = value + "\n" + textBlock.Text;
                }));


            }
        }

       
        ConnectionMultiplexer redis = null;

        IDatabase db = null;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Sfigure", "Session", "hi Sfigure");

            string Session = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Sfigure", "Session", null);
            if (Session != null)
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Sfigure", "Session", "hi Sfigure");
            }
            RedisHostConnect.IsEnabled = true;
            RedisHostDisconnect.IsEnabled = false;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private void Button_Redis_Disconnect(object sender, RoutedEventArgs e) {
            RedisHostConnect.IsEnabled = true;
            RedisHostDisconnect.IsEnabled = false;
            redis.CloseAsync();
        }

        private void KeyItemMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem target = (TreeViewItem)sender;
            String key = target.Header.ToString();
            string[] a = { key };
            RedisResult redisResult= db.Execute("get", a );
            parseResult(redisResult);
        }
        private void  SessionTreeItemMouseDoubleClick(object sender, RoutedEventArgs e) {
            MessageBox.Show("双击");
        }
        /// <summary>
        /// 连接
        /// </summary>
        private void Button_Redis_Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                if (redis.IsConnecting)
                {
                    redis.Dispose();
                }
            }
            catch { 
            }
            //重新初始化
            try {
                var log = new MainWindow.TextWriter1();
                log.textBlock = SessionLog;
                ConfigurationOptions configurationOptions = new ConfigurationOptions();
                configurationOptions.ReconnectRetryPolicy= new LinearRetry(5000);

                
                redis = ConnectionMultiplexer.Connect(RedisHost.Text, log);
                db = redis.GetDatabase();
                RedisHostConnect.IsEnabled = false;
                RedisHostDisconnect.IsEnabled = true;

                TreeViewItem a = new TreeViewItem();

                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;

                // create Image
                Image image = new Image();
                image.Source = new BitmapImage
                    (new Uri("pack://application:,,/Images/" + "redis.ico"));
                image.Width = 16;
                image.Height = 16;
                // Label
                Label lbl = new Label();
                lbl.Content = RedisHost.Text;


                // Add into stack
                stack.Children.Add(image);
                stack.Children.Add(lbl);
                a.Header =stack;

                //增加
                //获取全部的key
                string[] argv = { "*" };
                var redisResult = db.Execute("keys", argv);

                foreach (KeyValuePair<string, RedisResult> entry in redisResult.ToDictionary())
                {
                   
                    TreeViewItem KeyItem = new TreeViewItem();
                    KeyItem.Header= entry.Value;
                    KeyItem.MouseDoubleClick += KeyItemMouseDoubleClick;
                    a.Items.Add(KeyItem);
                }
                SessionTree.Items.Add(a);
            }
            catch (Exception ex) {
                SessionLog.Text = ex.Message.ToString();
            }
          
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public static String[] SubArray<String>(String[]  data, int index, int length)
        {
            String[] result = new String[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private void RedisCommandInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RedisCommandExec_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private  void parseResult(RedisResult redisResult) {
            RedisResult.Text = redisResult.Type.ToString();
            switch (redisResult.Type)
            {
                case ResultType.BulkString:
                    {
                        RedisResult.Text = redisResult.ToString();
                        break;
                    }
                case ResultType.SimpleString:
                    {
                        RedisResult.Text = redisResult.ToString();
                        break;
                    }
                case ResultType.MultiBulk:
                    {
                        foreach (KeyValuePair<string, RedisResult> entry in redisResult.ToDictionary())
                        {
                            RedisResult.Text = RedisResult.Text + "\n" + entry.Value;
                        }
                        break;
                    }
                case ResultType.Integer:
                    {
                        RedisResult.Text = redisResult.ToString();
                        break;
                    }
                case ResultType.Error:
                    {
                        RedisResult.Text = redisResult.ToString();
                        break;
                    }
                case ResultType.None:
                    {
                        RedisResult.Text = "没有返回值";
                        break;
                    }
            }
        }

        private void RedisCommandExec_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return) {
                try
                {
                    string[] args = RedisCommandInput.Text.Split(' ');
                    string[] arg1 = SubArray(args, 1, args.Length - 1);
                    RedisResult redisResult = db.Execute(args[0], arg1);
                    parseResult(redisResult);
                   
                }
                catch(Exception ex)
                {
                    SessionLog.Text = ex.Message.ToString();
                }
            }
        }
    }
}
