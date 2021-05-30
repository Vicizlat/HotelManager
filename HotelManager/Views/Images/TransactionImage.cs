using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HotelManager.Views.Images
{
    public class TransactionImage : Image
    {
        public TransactionImage()
        {
            ToolTip = "Manage transactions for this reservation";
            MouseEnter += Image_MouseEnter;
            MouseLeave += Image_MouseLeave;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\TransactionIconC.png");
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Source = ImageSource(@"Images\TransactionIconBW.png");
        }

        private static BitmapImage ImageSource(string uriString)
        {
            Uri imageUri = new Uri(uriString, UriKind.Relative);
            return new BitmapImage(imageUri);
        }
    }
}