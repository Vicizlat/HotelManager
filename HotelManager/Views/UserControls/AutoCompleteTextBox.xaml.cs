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
        public event EventHandler<string> OnSelectionChanged;
        public List<string> AutoSuggestionList { get; set; }
        public bool ReadOnly { get; set; }
        public bool SelectionChanged = false;

        public AutoCompleteTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AutoSuggestionPopup(bool openPopup)
        {
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
                    .Where(s => s.ToLower().Contains(autoText))
                    .OrderBy(x => x).Select(x => x);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in AutoTextBox_TextChanged", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AutoSuggestionPopup(false);
                if (AutoList.SelectedIndex < 0) return;
                SelectionChanged = true;
                OnSelectionChanged?.Invoke(this, $"{AutoList.SelectedItem}");
                AutoList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in AutoList_SelectionChanged", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}