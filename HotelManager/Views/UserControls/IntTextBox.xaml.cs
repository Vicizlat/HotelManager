using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManager.Views.UserControls
{
    public partial class IntTextBox
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public string Label { get; set; }
        public int MaxLength { get; set; }
        public int IntValue;

        public IntTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool Validate()
        {
            bool isValid = !string.IsNullOrEmpty(IntBox.Text) && int.TryParse(IntBox.Text, out IntValue) && IntValue > 0;
            IntBox.Background = isValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);
            IntBox.Foreground = isValid ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
            TextBlock.Foreground = isValid ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
            return isValid;
        }

        private void Numbers_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 &&
                e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 &&
                e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 && e.Key != Key.NumPad3 &&
                e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 && e.Key != Key.NumPad7 &&
                e.Key != Key.NumPad8 && e.Key != Key.NumPad9) e.Handled = true;
        }

        private void IntBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}