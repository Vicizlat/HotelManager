using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class PriceRangeCheckBox : CheckBox
    {
        private MainController controller;
        private int priceRangeId;

        public PriceRangeCheckBox(MainController controller, int priceRangeId)
        {
            this.controller = controller;
            this.priceRangeId = priceRangeId;
            IsChecked = controller.GetPriceRangeState(priceRangeId);
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Center;
            Checked += PriceRangeCheckBox_Checked;
            Unchecked += PriceRangeCheckBox_Unchecked;
        }

        private void PriceRangeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (controller.ValidatePriceRangeById(priceRangeId)) controller.UpdatePriceRangeState(priceRangeId, true);
            else IsChecked = false;
        }

        private void PriceRangeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            controller.UpdatePriceRangeState(priceRangeId, false);
        }
    }
}