using System.Windows;

namespace HotelManager.Views
{
    public partial class WaitWindow : Window
    {
        public WaitWindow(string text)
        {
            InitializeComponent();
            TextBlock.Text = text;
            Show();
        }
    }
}