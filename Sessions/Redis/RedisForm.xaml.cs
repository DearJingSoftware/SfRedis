using System;
using System.Linq;
using System.Windows;

namespace SfRedis.Sessions.Redis
{
    /// <summary>
    /// RedisForm.xaml 的交互逻辑
    /// </summary>
    public partial class RedisForm : Window
    {
        RedisSession redisSession = new RedisSession();
        public RedisForm()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            this.DataContext = redisSession;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int count= MainWindow.sessions.Where(x => x.GetIdentifier().Equals(redisSession.Name)).Count();
            if (count > 0) {
                MessageBox.Show("名称不能重复");
                return;
            }
            MainWindow.sessions.Add(redisSession);
            //当前连接
            MainWindow.ctxSession = redisSession;
            redisSession.Connect();
            this.Close();
        }

        void AsyncCallback() { 
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                redisSession.Connect();
                if (redisSession.IsConnected)
                {
                    MessageBox.Show("连接成功");
                }
            }
            catch (Exception ex) {
                MessageBox.Show("连接异常"+ex.Message);
            }
          
        }
    }
}
