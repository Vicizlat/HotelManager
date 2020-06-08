using System.Windows;
using System.Windows.Input;

namespace HotelManager
{
    public partial class SelectWebsiteWindow : Window
    {
        public SelectWebsiteWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Address_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Close();
            }
        }
    }
}