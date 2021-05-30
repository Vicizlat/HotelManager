using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Data.Models.Enums;
using HotelManager.Views.Templates;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Views
{
    public partial class SearchWindow
    {
        private readonly MainController controller;

        public SearchWindow(MainController controller)
        {
            InitializeComponent();
            this.controller = controller;
            SearchIn.SelectedIndex = 0;
            controller.OnReservationsChanged += UpdateResults;
        }

        private void UpdateResults(object sender, EventArgs e)
        {
            SearchButton_Click(sender, null);
        }

        private void SearchIn_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchIn.SelectedIndex == 0 || SearchIn.SelectedIndex == 1)
            {
                SearchBox.Visibility = Visibility.Visible;
                SearchStartDateText.Visibility = Visibility.Hidden;
                SearchStartDate.Visibility = Visibility.Hidden;
                SearchEndDateText.Visibility = Visibility.Hidden;
                SearchEndDate.Visibility = Visibility.Hidden;
            }
            else
            {
                SearchBox.Visibility = Visibility.Hidden;
                SearchStartDateText.Visibility = Visibility.Visible;
                SearchStartDate.Visibility = Visibility.Visible;
                SearchEndDateText.Visibility = Visibility.Visible;
                SearchEndDate.Visibility = Visibility.Visible;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsTable.Children.Clear();
            ResultsTable.RowDefinitions.Clear();
            bool includeCanceled = IncludeCanceled.IsChecked ?? false;
            DateTime? searchStartDate = SearchStartDate.SelectedDate;
            DateTime? searchEndDate = SearchEndDate.SelectedDate;
            Dictionary<int, string> results = SearchIn.SelectedIndex switch
            {
                0 => controller.Context.Reservations.Where(r => includeCanceled || r.State != State.Canceled)
                    .Where(r => (r.Guest.FirstName + r.Guest.LastName).ToLower().Contains(SearchBox.Text.ToLower()))
                    .Include(r => r.Guest).OrderBy(r => r.Id)
                    .Select(r => new { r.Id, res = r.ToString() }).ToDictionary(arg => arg.Id, arg => arg.res),
                1 => controller.Context.Reservations.Where(r => includeCanceled || r.State != State.Canceled)
                    .Where(r => r.Notes.ToLower().Contains(SearchBox.Text.ToLower()))
                    .Include(r => r.Guest).OrderBy(r => r.Id)
                    .Select(r => new { r.Id, res = r.ToString() }).ToDictionary(arg => arg.Id, arg => arg.res),
                2 => controller.Context.Reservations.Where(r => includeCanceled || r.State != State.Canceled)
                    .Where(r => r.StartDate >= searchStartDate && r.StartDate <= searchEndDate)
                    .Include(r => r.Guest).OrderBy(r => r.Id)
                    .Select(r => new { r.Id, res = r.ToString() }).ToDictionary(arg => arg.Id, arg => arg.res),
                3 => controller.Context.Reservations.Where(r => includeCanceled || r.State != State.Canceled)
                    .Where(r => r.EndDate >= searchStartDate && r.EndDate <= searchEndDate)
                    .Include(r => r.Guest).OrderBy(r => r.Id)
                    .Select(r => new { r.Id, res = r.ToString() }).ToDictionary(arg => arg.Id, arg => arg.res),
                4 => controller.Context.Reservations.Where(r => includeCanceled || r.State != State.Canceled)
                    .Where(r => r.StartDate >= searchStartDate && r.EndDate <= searchEndDate)
                    .Include(r => r.Guest).OrderBy(r => r.Id)
                    .Select(r => new { r.Id, res = r.ToString() }).ToDictionary(arg => arg.Id, arg => arg.res),
                _ => new Dictionary<int, string>()
            };
            decimal reservationsSum = controller.Context.Reservations.Where(r => results.Keys.Contains(r.Id)).Sum(r => r.TotalSum);
            SearchResults.Text = $"Обща сума на резервациите: {reservationsSum} | ";
            SearchResults.Text += $"Намерени резултати: {results.Count}";
            for (int i = 0; i < results.Count; i++)
            {
                ResultsTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string[] resStrings = results.ElementAt(i).Value.Split(" | ", StringSplitOptions.TrimEntries);
                TextBlock resId = new TextBlock
                {
                    Text = resStrings[0].Substring(7),
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(resId, 0);
                Grid.SetRow(resId, i);
                TextBlock resState = new TextBlock
                {
                    Text = resStrings[1].Substring(11),
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(resState, 1);
                Grid.SetRow(resState, i);
                TextBlock resSource = new TextBlock
                {
                    Text = resStrings[2].Substring(10),
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(resSource, 2);
                Grid.SetRow(resSource, i);
                TextBlock resRoom = new TextBlock
                {
                    Text = resStrings[3].Substring(6),
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(resRoom, 3);
                Grid.SetRow(resRoom, i);
                string reservationString = string.Join(" | ", resStrings.Skip(4));
                TextBox tableTextBox = new ReservationTextBox(controller, results.ElementAt(i).Key, reservationString);
                Grid.SetColumn(tableTextBox, 4);
                Grid.SetRow(tableTextBox, i);
                ResultsTable.Children.Add(resId);
                ResultsTable.Children.Add(resState);
                ResultsTable.Children.Add(resSource);
                ResultsTable.Children.Add(resRoom);
                ResultsTable.Children.Add(tableTextBox);
            }
        }
    }
}