
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
using System.Windows.Data;
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

            sessions.CollectionChanged += Sessions_CollectionChanged;
            this.SessionTree.ItemsSource = sessions;

        }

        private void Sessions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //MessageBox.Show("更新数据");
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private void Button_Redis_Disconnect(object sender, RoutedEventArgs e)
        {
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

        private void TreeViewItem_Key_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           MessageBox.Show(((SfRedis.Sessions.SfRedisKey)(((TreeViewItem)sender).DataContext)).Name);
        }

        private void Menu_Keys_Refresh(object sender, RoutedEventArgs e)
        {

        }
        private void Menu_Keys_Remove(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result2 = System.Windows.MessageBox.Show("确认删除?", "确认", MessageBoxButton.OKCancel);
            if (result2.Equals(MessageBoxResult.OK))
            {
                SfRedis.Sessions.SfRedisKey key = ((SfRedis.Sessions.SfRedisKey)(((System.Windows.Controls.MenuItem)sender).DataContext));
                key.Session.Command("del " + key.Name);
                key.Session.Refresh(key.Session);
            }
        }

        private void App_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Redis_Session_Disconnect(object sender, RoutedEventArgs e)
        {
            ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.MenuItem)sender).DataContext)).DisConnect();
            CollectionViewSource.GetDefaultView(sessions).Refresh();

        }

        private void Redis_Session_ReConnect(object sender, RoutedEventArgs e)
        {
            ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.MenuItem)sender).DataContext)).ReConnect();
            CollectionViewSource.GetDefaultView(sessions).Refresh();
        }

        private void Redis_Session_Connect(object sender, RoutedEventArgs e)
        {
            ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.MenuItem)sender).DataContext)).Connect();
            CollectionViewSource.GetDefaultView(sessions).Refresh();
        }

        private void Redis_Session_Connect(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            //{
            //    ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.StackPanel)sender).DataContext)).Connect();
            //    CollectionViewSource.GetDefaultView(sessions).Refresh();
               
            //}
            //e.Handled = false;


        }
    }
}
