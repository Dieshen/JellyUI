using System.Windows;
using System.Windows.Controls;

namespace CS_Jelly
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("CS_Jelly", "Button clicked");
        }
    }
}