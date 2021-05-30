using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManager.Views.Templates.HotelSetup
{
    public class ItemTextBox : TextBox
    {
        public ItemTextBox(int id, string text)
        {
            Name = $"TId{id}";
            IsReadOnly = true;
            FontSize = 18;
            Padding = new Thickness(2);
            Cursor = Cursors.Hand;
            Text = text;
        }
    }
}