using System.Windows;
using System.Windows.Controls;

namespace HotelManager.Views.Templates
{
    public class PriceRangeTextBox : TextBox
    {
        public PriceRangeTextBox(string text)
        {
            Text = text;
            FontSize = 14;
            IsReadOnly = true;
            Focusable = false;
            Padding = new Thickness(2);
        }
    }
}