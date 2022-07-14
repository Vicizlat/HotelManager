using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class ReservationIcon : Image
    {
        public string ImageUri;

        public ReservationIcon(int sourceInt)
        {
            string[] imageNames = new[] { "Phone", "Mail", "Booking", "HotelIconC", "Friend" };
            ImageUri = @$"Images\{imageNames[sourceInt]}.png";
            Uri imageUri = new Uri(ImageUri, UriKind.Relative);
            Source = new BitmapImage(imageUri);
            VerticalAlignment = VerticalAlignment.Top;
            Height = 30;
        }
    }
}