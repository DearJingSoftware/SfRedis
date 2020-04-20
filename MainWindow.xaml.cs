using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                a.Header = "测试";
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

        private void RedisCommandExec_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return) {
                try
                {
                    String[] args = RedisCommandInput.Text.Split(' ');
                    String[] arg1 = SubArray(args, 1, args.Length - 1);
                    RedisResult redisResult = db.Execute(args[0], arg1);
                    RedisResult.Text = redisResult.Type.ToString();
                    switch (redisResult.Type) {
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
                        case ResultType.Integer: {
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
                catch(Exception ex)
                {
                    SessionLog.Text = ex.Message.ToString();
                }
            }
        }
    }
}
