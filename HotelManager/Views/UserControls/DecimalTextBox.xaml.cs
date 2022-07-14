using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManager.Views.UserControls
{
    public partial class DecimalTextBox
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public string Label { get; set; }
        public bool ReadOnly { get; set; } = false;
        public bool HitTestVisible { get; set; } = true;
        public bool IsFocusable { get; set; } = true;
        public decimal DecimalValue;

        public DecimalTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool Validate(bool allowNegative = false)
        {
            bool isDecimal = decimal.TryParse(DecimalBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out DecimalValue);
            bool isValid = !string.IsNullOrEmpty(DecimalBox.Text) && isDecimal && (allowNegative || DecimalValue >= 0);
            DecimalBox.Background = isValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);
            DecimalBox.Foreground = isValid ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
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
            bool keyDecimal = e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Decimal;
            bool keyMinus = e.Key == Key.OemMinus || e.Key == Key.Subtract;
            if (keyDecimal && !DecimalBox.Text.Contains('.')) InsertText(DecimalBox.SelectionStart, ".");
            if (keyMinus)
            {
                if (!DecimalBox.Text.Contains('-')) InsertText(DecimalBox.SelectionStart, "-", 0);
                else
                {
                    int caretPosition = DecimalBox.SelectionStart;
                    DecimalBox.Text = DecimalBox.Text.Replace("-", string.Empty);
                    DecimalBox.SelectionStart = caretPosition - 1;
                }
            }
        }

        private void InsertText(int caretPosition, string insertText, int? insertPosition = null)
        {
            DecimalBox.Text = DecimalBox.Text.Insert(insertPosition ?? caretPosition, insertText);
            DecimalBox.SelectionStart = caretPosition + 1;
        }

        private void DecimalBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DecimalBox.Text = DecimalBox.Text.Replace(',', '.');
            TextChanged?.Invoke(sender, e);
        }
    }
}