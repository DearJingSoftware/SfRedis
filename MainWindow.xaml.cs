using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        ConnectionMultiplexer redis = null;

        IDatabase db = null;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private void Button_Redis_Disconnect(object sender, RoutedEventArgs e) {
            redis.CloseAsync();
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
                redis = ConnectionMultiplexer.Connect(RedisHost.Text);
                db = redis.GetDatabase();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
          
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RedisHostTest_Click(object sender, RoutedEventArgs e)
        {
            db.StringSet("test", "c# redis");
            MessageBox.Show(db.StringGet("test"));
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
                    MessageBox.Show(redisResult.ToString());
                }
                catch
                {

                }
            }
        }
    }
}
