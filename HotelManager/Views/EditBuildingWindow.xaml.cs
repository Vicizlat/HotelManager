using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Data.Models;
using HotelManager.Views.Templates.HotelSetup;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Views
{
    public partial class EditBuildingWindow
    {
        public MainController Controller { get; }
        private Building Building { get; }
        public EditBuildingWindow(MainController controller, Building building)
        {
            InitializeComponent();
            Controller = controller;
            Building = building;
            BuildingName.Text = building.BuildingName;
            CreateRows();
        }

        private void CreateRows()
        {
            Floors.Children.Clear();
            Floors.RowDefinitions.Clear();
            foreach (Floor floor in Controller.Context.Floors.Include(f => f.Rooms).Where(f => f.Building == Building))
            {
                Floors.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string text = $"Етаж {floor.FloorNumber} - {floor.Rooms.Count} стаи";
                TextBox floorTextBox = new ItemTextBox(floor.Id, text);
                floorTextBox.MouseDoubleClick += EditFloorOnMouseDoubleClick;
                Button removeFloorButton = new RemoveButton(floor.Id, RemoveType.Floor);
                removeFloorButton.Click += RemoveFloorButtonOnClick;
                Grid.SetRow(floorTextBox, Floors.RowDefinitions.Count - 1);
                Grid.SetRow(removeFloorButton, Floors.RowDefinitions.Count - 1);
                Floors.Children.Add(floorTextBox);
                Floors.Children.Add(removeFloorButton);
            }
        }

        private void AddFloorButtonOnClick(object sender, RoutedEventArgs e)
        {
            ((HotelSetupWindow)Owner).SaveBuildings(Building);
            int lastFloorNumber = Building.Floors.Any() ? Building.Floors.Last().FloorNumber : -1;
            Floor floor = new Floor { FloorNumber = lastFloorNumber + 1, Building = Building };
            EditFloorWindow editFloorWindow = new EditFloorWindow(Controller, floor) { Owner = this };
            Hide();
            editFloorWindow.ShowDialog();
            ShowDialog();
        }

        private void EditFloorOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Floor floor = GetFloor(sender);
            if (floor == null) return;
            EditFloorWindow editFloorWindow = new EditFloorWindow(Controller, floor) { Owner = this };
            Hide();
            editFloorWindow.ShowDialog();
            ShowDialog();
        }

        private void RemoveFloorButtonOnClick(object sender, RoutedEventArgs e)
        {
            Floor floor = GetFloor(sender);
            if (floor == null || floor.Rooms.Any()) return;
            Building.Floors.Remove(floor);
            SaveFloors(null);
        }

        private Floor GetFloor(object sender)
        {
            int id = int.Parse(((Control)sender).Name.Substring(3));
            return Controller.Context.Floors.Include(f => f.Rooms).FirstOrDefault(f => f.Id == id);
        }

        private void BuildingName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Save.IsEnabled = !string.IsNullOrEmpty(BuildingName.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Building.BuildingName = BuildingName.Text;
            ((HotelSetupWindow)Owner).SaveBuildings(Building);
            Close();
        }

        public void SaveFloors(Floor floor)
        {
            if (floor != null && !Controller.Context.Floors.Contains(floor))
            {
                Controller.Context.Floors.Add(floor);
            }
            Controller.Context.SaveChanges();
            CreateRows();
        }
    }
}