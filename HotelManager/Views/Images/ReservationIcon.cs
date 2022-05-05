using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class ReservationIcon : Image
    {
        public ReservationIcon(int sourceInt)
        {
            string[] imageNames = new[] { "Phone", "Mail", "Booking", "HotelIconC", "Friend" };
            Uri imageUri = new Uri(@$"Images\{imageNames[sourceInt]}.png", UriKind.Relative);
            Source = new BitmapImage(imageUri);
            VerticalAlignment = VerticalAlignment.Top;
            Height = 30;
        }
    }
}