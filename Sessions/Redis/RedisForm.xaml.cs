﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            InitializeComponent();
            this.DataContext = redisSession;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.sessions.Add(redisSession);
        }
    }
}