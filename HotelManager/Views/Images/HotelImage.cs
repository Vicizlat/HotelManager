using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class HotelImage : Image
    {
        public HotelImage()
        {
            ToolTip = "Hotel Setup (Buildings, Floors, Rooms)";
            MouseEnter += Image_MouseEnter;
            MouseLeave += Image_MouseLeave;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\HotelIconC.png");
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\HotelIconBW.png");
        }

        private static BitmapImage ImageSource(string uriString)
        {
            Uri imageUri = new Uri(uriString, UriKind.Relative);
            return new BitmapImage(imageUri);
        }
    }
}