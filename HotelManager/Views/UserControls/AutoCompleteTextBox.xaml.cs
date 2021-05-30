using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelManager.Views.UserControls
{
    public partial class AutoCompleteTextBox
    {
        public IEnumerable<string[]> AutoSuggestionList { get; set; }

        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }

        private void AutoSuggestionPopup(bool openPopup)
        {
            AutoListPopup.Width = ActualWidth;
            AutoListPopup.IsOpen = openPopup;
            AutoListPopup.Visibility = openPopup ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool Validate()
        {
            bool isValid = !string.IsNullOrEmpty(AutoTextBox.Text);
            AutoTextBox.Background = isValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);
            return isValid;
        }

        private void AutoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (AutoTextBox.Text.Length < 3)
                {
                    AutoSuggestionPopup(false);
                    return;
                }
                AutoSuggestionPopup(true);
                string autoText = AutoTextBox.Text.ToLower();
                AutoList.ItemsSource = AutoSuggestionList
                    .Where(s => s[0].ToLower().Contains(autoText))
                    .OrderBy(x => x[0]).Select(x => x[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AutoSuggestionPopup(false);
                if (AutoList.SelectedIndex < 0) return;
                AutoTextBox.Text = $"{AutoList.SelectedItem}";
                AutoList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}