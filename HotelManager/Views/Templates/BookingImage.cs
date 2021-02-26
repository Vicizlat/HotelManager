using System;
using System.Net.Cache;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Templates
{
    public class BookingImage : Image
    {
        public BookingImage()
        {
            Uri imageUri = new Uri("https://account.booking.com/favicon.ico", UriKind.RelativeOrAbsolute);
            BitmapImage imageSource = new BitmapImage(imageUri, new RequestCachePolicy(RequestCacheLevel.Default));
            Source = imageSource;
            VerticalAlignment = VerticalAlignment.Top;
            Height = 30;
        }
    }
}