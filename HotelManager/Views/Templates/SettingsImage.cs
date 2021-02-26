using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class SettingsImage : Image
    {
        private readonly IController controller;
        public SettingsImage(IController controller)
        {
            this.controller = controller;
            Source = ImageSource(@"Templates\settings-gear-icon-plain.png");
            Height = 50;
            Width = 50;
            Cursor = Cursors.Hand;
            MouseEnter += SettingsImage_MouseEnter;
            MouseLeave += SettingsImage_MouseLeave;
            MouseLeftButtonDown += SettingsImage_MouseLeftButtonDown;
        }

        private void SettingsImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Templates\settings-gear-icon.png");
        }

        private void SettingsImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Templates\settings-gear-icon-plain.png");
        }

        private void SettingsImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            controller.ShowSettingsWindow();
        }

        private BitmapImage ImageSource(string uriString)
        {
            Uri imageUri = new Uri(uriString, UriKind.Relative);
            return new BitmapImage(imageUri);
        }
    }
}