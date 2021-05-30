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
    public partial class EditFloorWindow
    {
        public MainController Controller { get; }
        private Floor Floor { get; }

        public EditFloorWindow(MainController controller, Floor floor)
        {
            InitializeComponent();
            Controller = controller;
            Floor = floor;
            FloorNumber.Text = $"{floor.FloorNumber}";
            CreateRows();
        }

        private void CreateRows()
        {
            Rooms.Children.Clear();
            Rooms.RowDefinitions.Clear();
            foreach (Room room in Controller.Context.Rooms.Include(r => r.Reservations).Where(r => r.Floor == Floor))
            {
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string text = $"{room} - {room.Reservations.Count} резервации";
                TextBox roomTextBox = new ItemTextBox(room.Id, text);
                roomTextBox.MouseDoubleClick += EditRoomOnMouseDoubleClick;
                Button removeRoomButton = new RemoveButton(room.Id, RemoveType.Room);
                removeRoomButton.Click += RemoveRoomButtonOnClick;
                Grid.SetRow(roomTextBox, Rooms.RowDefinitions.Count - 1);
                Grid.SetRow(removeRoomButton, Rooms.RowDefinitions.Count - 1);
                Rooms.Children.Add(roomTextBox);
                Rooms.Children.Add(removeRoomButton);
            }
        }

        private void AddRoomButtonOnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("EditRoomWindow is not created!");
            ((EditBuildingWindow)Owner).SaveFloors(Floor);
            bool hasRooms = Floor.Rooms.Any();
            int lastRoomNumber = hasRooms ? Floor.Rooms.Last().RoomNumber : 0;
            Room room = new Room
            {
                Floor = Floor,
                RoomNumber = lastRoomNumber + 1,
                FullRoomNumber = int.Parse($"{Floor.FloorNumber}{lastRoomNumber + 1}"),
                MaxGuests = 4,
                FirstOnFloor = !hasRooms,
                LastOnFloor = false
            };
            Controller.Context.Rooms.Add(room);
            Controller.Context.SaveChanges();
            CreateRows();
        }

        private void EditRoomOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("EditRoomWindow is not created!");
        }

        private void RemoveRoomButtonOnClick(object sender, RoutedEventArgs e)
        {
            Room room = GetRoom(sender);
            if (room == null || room.Reservations.Any()) return;
            Floor.Rooms.Remove(room);
            SaveRooms(null);
        }

        private Room GetRoom(object sender)
        {
            int id = int.Parse(((Control)sender).Name.Substring(3));
            return Controller.Context.Rooms.Include(r => r.Reservations).FirstOrDefault(r => r.Id == id);
        }

        private void FloorNumber_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]);
        }

        private void FloorNumber_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Save.IsEnabled = !string.IsNullOrEmpty(FloorNumber.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Floor.FloorNumber = int.Parse(FloorNumber.Text);
            if (Floor.Rooms.Any()) Floor.Rooms.Last().LastOnFloor = true;
            ((EditBuildingWindow)Owner).SaveFloors(Floor);
            Close();
        }

        public void SaveRooms(Room room)
        {
            if (room != null && !Controller.Context.Rooms.Contains(room))
            {
                Controller.Context.Rooms.Add(room);
            }
            Controller.Context.SaveChanges();
            CreateRows();
        }
    }
}