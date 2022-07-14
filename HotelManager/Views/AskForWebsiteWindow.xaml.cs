using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Handlers;
using HotelManager.Utils;

namespace HotelManager.Program.Views
{
    public partial class AskForWebsiteWindow : Window
    {
        public AskForWebsiteWindow()
        {
            InitializeComponent();
            WebsiteAddress.Focus();
        }

        private void WebsiteAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!WebsiteAddress.Text.StartsWith("https://"))
            {
                WebsiteAddress.Text = WebsiteAddress.Text.Insert(0, "https://");
                WebsiteAddress.CaretIndex = WebsiteAddress.Text.Length;
            }
            Save.IsEnabled = Validate();
        }

        private bool Validate()
        {
            bool notEmptyString = !string.IsNullOrEmpty(WebsiteAddress.Text);
            bool noSpacesInString = !WebsiteAddress.Text.Contains(' ');
            bool isValid = notEmptyString && noSpacesInString;
            WebsiteAddress.Background = isValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);
            WebsiteAddress.Foreground = isValid ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
            return isValid;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            bool result = await WebHandler.TryGetFileAsync(WebsiteAddress.Text, Constants.SettingsFilename);
            if (result)
            {
                DialogResult = result;
                Close();
            }
            else
            {
                WebsiteAddress.Background = new SolidColorBrush(Colors.Bisque);
                WebsiteAddress.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void WebsiteAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Validate())
            {
                Save_Click(this, null);
            }
        }
    }
}