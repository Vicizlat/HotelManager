using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Views.Templates;

namespace HotelManager.Views
{
    public partial class PriceRangesWindow
    {
        private MainController controller;

        public PriceRangesWindow(MainController controller)
        {
            InitializeComponent();
            this.controller = controller;
            DisplayPriceRanges();
        }

        private void AddPriceRange_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = StartDate.SelectedDate.Value;
            DateTime endDate = EndDate.SelectedDate.Value;
            decimal basePrice = BasePrice.DecimalValue;
            int baseGuests = BasePriceGuests.IntValue;
            decimal priceChange = PriceChangePerGuest.DecimalValue;
            controller.AddPriceRange(startDate, endDate, basePrice, baseGuests, priceChange);
            DisplayPriceRanges();
            AddPriceRange.IsEnabled = IsSaveEnabled();
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPriceRange.IsEnabled = IsSaveEnabled();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddPriceRange.IsEnabled = IsSaveEnabled();
        }

        private void DisplayPriceRanges()
        {
            PriceRanges.Children.Clear();
            PriceRanges.RowDefinitions.Clear();
            List<string> priceRangesList = controller.GetPriceRangeStrings();
            for (int i = 0; i < priceRangesList.Count; i++)
            {
                PriceRanges.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string[] priceRangeStrings = priceRangesList[i].Split(" | ", StringSplitOptions.TrimEntries);
                for (int j = 1; j < priceRangeStrings.Length; j++)
                {
                    PriceRangeTextBox textBox = new PriceRangeTextBox(priceRangeStrings[j]);
                    Grid.SetColumn(textBox, j - 1);
                    Grid.SetRow(textBox, i);
                    PriceRanges.Children.Add(textBox);
                }
                PriceRangeCheckBox checkBox = new PriceRangeCheckBox(controller, i + 1);
                Grid.SetColumn(checkBox, priceRangeStrings.Length - 1);
                Grid.SetRow(checkBox, i);
                PriceRanges.Children.Add(checkBox);
            }
        }

        private bool IsSaveEnabled()
        {
            bool validStartDate = StartDate.SelectedDate != null && !controller.ValidatePriceRangeByDate(StartDate.SelectedDate.Value);
            bool validEndDate = EndDate.SelectedDate != null && !controller.ValidatePriceRangeByDate(EndDate.SelectedDate.Value);
            bool validBasePrice = BasePrice.Validate();
            bool validBasePriceGuests = BasePriceGuests.Validate();
            bool validPriceChangePerGuest = PriceChangePerGuest.Validate();
            return validStartDate && validEndDate && validBasePrice && validBasePriceGuests && validPriceChangePerGuest;
        }
    }
}