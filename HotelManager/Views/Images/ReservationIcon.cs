using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class ReservationIcon : Image
    {
        public ReservationIcon(string imageName)
        {
            Uri imageUri = new Uri(@$"Images\{imageName}.png", UriKind.Relative);
            Source = new BitmapImage(imageUri);
            VerticalAlignment = VerticalAlignment.Top;
            Height = 30;
        }
    }
}