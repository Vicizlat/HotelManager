using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class SettingsImage : Image
    {
        public SettingsImage()
        {
            ToolTip = "Settings";
            MouseEnter += SettingsImage_MouseEnter;
            MouseLeave += SettingsImage_MouseLeave;
            MouseLeftButtonDown += SettingsImage_MouseLeftButtonDown;
        }

        private void SettingsImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\SettingsIconC.png");
        }

        private void SettingsImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\SettingsIconBW.png");
        }

        private void SettingsImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private BitmapImage ImageSource(string uriString)
        {
            Uri imageUri = new Uri(uriString, UriKind.Relative);
            return new BitmapImage(imageUri);
        }
    }
}