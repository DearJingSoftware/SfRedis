using System.Windows;

namespace SfRedis.Windows
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("check update");
        }
    }
}