
using SfRedis.Sessions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace SfRedis
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        class TextWriter1 : TextWriter
        {
            internal System.Windows.Controls.TextBox textBlock;

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

        public static ObservableCollection<Session> sessions = new ObservableCollection<Session> { };
        public static Session ctxSession =null;

        ConnectionMultiplexer redis = null;

        IDatabase db = null;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            RedisHostConnect.IsEnabled = true;
            RedisHostDisconnect.IsEnabled = false;
            this.SessionTree.ItemsSource = sessions;

        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private void Button_Redis_Disconnect(object sender, RoutedEventArgs e)
        {
            RedisHostConnect.IsEnabled = true;
            RedisHostDisconnect.IsEnabled = false;
            redis.CloseAsync();
        }

        private void KeyItemMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem target = (TreeViewItem)sender;
            String key = target.Header.ToString();
            RedisResult redisResult = FetchKey(key);
            ParseResult(redisResult);
        }

        private RedisResult FetchKey(string key)
        {

            RedisType redisKeyTypeResult = db.KeyType(new RedisKey(key));
            String fetch = null;
            List<string> para = new List<string>();
            switch (redisKeyTypeResult)
            {
                case RedisType.None:
                    fetch = null;
                    break;
                case RedisType.String:
                    fetch = "GET";
                    para.Add(key);
                    break;
                case RedisType.Set:
                    fetch = "SMEMBERS";
                    para.Add(key);
                    break;
                case RedisType.Hash:
                    fetch = "HGETALL";
                    //MessageBox.Show("HGETALL");
                    para.Add(key);
                    break;
                default:
                    throw new NotImplementedException();
            }

            RedisResult redisResult = db.Execute(fetch, para.ToArray());
            return redisResult;
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void Button_Redis_Connect(object sender, RoutedEventArgs e)
        {


            //try
            //{
            //    var log = new MainWindow.TextWriter1();
            //    log.textBlock = SessionLog;
            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.ReconnectRetryPolicy = new LinearRetry(5000);


            redis = ConnectionMultiplexer.Connect(RedisHost.Text);
            db = redis.GetDatabase();
            RedisHostConnect.IsEnabled = false;
            RedisHostDisconnect.IsEnabled = true;

            //    TreeViewItem a = new TreeViewItem();

            //    StackPanel stack = new StackPanel();
            //    stack.Orientation = Orientation.Horizontal;

            //    // create Image
            //    Image image = new Image();
            //    image.Source = new BitmapImage
            //        (new Uri("pack://application:,,/Images/redis.ico"));
            //    image.Width = 16;
            //    image.Height = 16;
            //    // Label
            //    Label lbl = new Label();
            //    lbl.Content = RedisHost.Text;


            //    // Add into stack
            //    stack.Children.Add(image);
            //    stack.Children.Add(lbl);
            //    a.Header = stack;

            //    //增加
            //    //获取全部的key
            //    string[] argv = { "*" };
            //    var redisResult = db.Execute("keys", argv);

            //    foreach (KeyValuePair<string, RedisResult> entry in redisResult.ToDictionary())
            //    {

            //        TreeViewItem KeyItem = new TreeViewItem();
            //        KeyItem.Header = entry.Value;
            //        KeyItem.MouseDoubleClick += KeyItemMouseDoubleClick;
            //        a.Items.Add(KeyItem);
            //    }
            //    SessionTree.Items.Add(a);
            //}
            //catch (Exception ex)
            //{
            //    SessionLog.Text = ex.Message.ToString();
            //}


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public static String[] SubArray<String>(String[] data, int index, int length)
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

        private void ParseResult(RedisResult redisResult)
        {
            RedisResult.Text = "";
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
                            RedisResult.Text = RedisResult.Text + "\n" + entry.Key + ":" + entry.Value;
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
            if (e.Key == System.Windows.Input.Key.Return)
            {
                try
                {
                    ((RedisSession)ctxSession).Command(RedisCommandInput.Text);
                }
                catch (Exception ex)
                {
                    SessionLog.Text = ex.Message.ToString();
                }
            }
        }

        private void Button_Redis_New(object sender, RoutedEventArgs e)
        {

            var redission = new RedisSession();
            redission.Create();
        }

        private void Redis_Session_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result2 = System.Windows.MessageBox.Show("确认删除?","确认",MessageBoxButton.OKCancel);
            if(result2.Equals(MessageBoxResult.OK))
            sessions.Remove((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.MenuItem)sender).DataContext));
        }

        private void TreeViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((SfRedis.Sessions.RedisSession)(((StackPanel)sender).DataContext)).Connect();
        }

        
    }
}
