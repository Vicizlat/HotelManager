using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManager.Views.Templates
{
    public class TransactionTextBox : TextBox
    {
        private TransactionsWindow transactionsWindow;

        public TransactionTextBox(TransactionsWindow transactionsWindow)
        {
            this.transactionsWindow = transactionsWindow;
            FontSize = 14;
            Cursor = Cursors.Arrow;
            Focusable = false;
            Padding = new Thickness(2);
            IsReadOnly = true;
            if (transactionsWindow != null)
            {
                FontWeight = FontWeights.Bold;
                TextDecorations = TextDecorationCollectionConverter.ConvertFromString("Underline");
                Foreground = Brushes.Blue;
                Cursor = Cursors.Hand;
                MouseUp += TransactionTextBox_MouseUp;
            }
        }

        private void TransactionTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            transactionsWindow.ReservationId.IntBox.Text = Text;
        }
    }
}