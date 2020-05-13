
using SfRedis.Sessions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SfRedis.Windows;

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

       

        private void RedisCommandExec_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           
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

        private void Click_Redis_Session_Connect(object sender, RoutedEventArgs e)
        {
            ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.MenuItem)sender).DataContext)).Connect();
            CollectionViewSource.GetDefaultView(sessions).Refresh();
        }

        private void Redis_Session_Connect(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.StackPanel)sender).DataContext)).Connect();
                CollectionViewSource.GetDefaultView(sessions).Refresh();

            }
            else
            {
                ctxSession = ((SfRedis.Sessions.RedisSession)(((System.Windows.Controls.StackPanel)sender).DataContext));
                // MessageBox.Show(ctxSession.GetIdentifier());
            }
            //e.Handled = false;


        }

        private void Redis_Session_DB_Switch(object sender, RoutedEventArgs e)
        {

        }

        private void Redis_Session_Copy(object sender, RoutedEventArgs e)
        {

        }

        private void Redis_Session_Rename(object sender, RoutedEventArgs e)
        {

        }

        private void Redis_Session_Exec(object sender, RoutedEventArgs e)
        {

        }

        private void Redis_Session_Refresh(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Redis_Search(object sender, RoutedEventArgs e)
        {

        }

        private void About_Windows(object sender, RoutedEventArgs e)
        { 
            new About().Show();
        }

        private void Check_Refresh_Windows(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void License_Windows(object sender, RoutedEventArgs e)
        {
            new License().Show();
        }
    }
}
