using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;
using HotelManager.Data.Models;
using HotelManager.Views.Templates.HotelSetup;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Views
{
    public partial class HotelSetupWindow
    {
        public MainController Controller { get; }
        public HotelSetupWindow(MainController controller)
        {
            InitializeComponent();
            Controller = controller;
            CreateRows();
        }

        private void CreateRows()
        {
            Buildings.Children.Clear();
            Buildings.RowDefinitions.Clear();
            int row = 0;
            foreach (Building building in Controller.Context.Buildings.Include(b => b.Floors))
            {
                Buildings.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                TextBox buildingTextBox = new ItemTextBox(building.Id, building.ToString());
                buildingTextBox.MouseDoubleClick += EditBuildingOnMouseDoubleClick;
                Button removeBuildingButton = new RemoveButton(building.Id, RemoveType.Building);
                removeBuildingButton.Click += RemoveBuildingButtonOnClick;
                Grid.SetRow(buildingTextBox, row);
                Grid.SetRow(removeBuildingButton, row);
                Buildings.Children.Add(buildingTextBox);
                Buildings.Children.Add(removeBuildingButton);
                row++;
            }
        }

        private void AddBuildingButtonOnClick(object sender, RoutedEventArgs e)
        {
            int lastBuildingNumber = Controller.Context.Buildings.OrderBy(b => b.Id).LastOrDefault()?.Id ?? 0;
            Building building = new Building { BuildingName = $"{lastBuildingNumber + 1}" };
            EditBuildingWindow editBuildingWindow = new EditBuildingWindow(Controller, building) { Owner = this };
            Hide();
            editBuildingWindow.ShowDialog();
            ShowDialog();
        }

        private void EditBuildingOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Building building = GetBuilding(sender);
            if (building == null) return;
            EditBuildingWindow editBuildingWindow = new EditBuildingWindow(Controller, building) { Owner = this };
            Hide();
            editBuildingWindow.ShowDialog();
            ShowDialog();
        }

        private void RemoveBuildingButtonOnClick(object sender, RoutedEventArgs e)
        {
            Building building = GetBuilding(sender);
            if (building == null || building.Floors.Any()) return;
            Controller.Context.Buildings.Remove(building);
            SaveBuildings(null);
        }

        private Building GetBuilding(object sender)
        {
            int id = int.Parse(((Control)sender).Name.Substring(3));
            return Controller.Context.Buildings.Include(b => b.Floors).FirstOrDefault(b => b.Id == id);
        }

        public void SaveBuildings(Building building)
        {
            if (building != null && !Controller.Context.Buildings.Contains(building))
            {
                Controller.Context.Buildings.Add(building);
            }
            Controller.Context.SaveChanges();
            CreateRows();
        }
    }
}