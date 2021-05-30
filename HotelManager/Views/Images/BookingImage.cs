﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class BookingImage : Image
    {
        public BookingImage()
        {
            Uri imageUri = new Uri(@"Images\Booking.ico", UriKind.Relative);
            Source = new BitmapImage(imageUri);
            VerticalAlignment = VerticalAlignment.Top;
            Height = 30;
        }
    }
}