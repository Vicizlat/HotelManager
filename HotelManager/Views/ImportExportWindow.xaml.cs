using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace HotelManager.Views
{
    public partial class ImportExportWindow
    {
        private readonly MainController controller;

        public ImportExportWindow(MainController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void ImportBrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                FileName = ImportFilePath.Text,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };
            if (dlg.ShowDialog() == true)
            {
                ImportFilePath.Text = dlg.FileName;
            }
            Import.IsEnabled = IsImportExportEnabled(ImportCollection, ImportFilePath.Text);
        }

        private void ImportCollection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Import.IsEnabled = IsImportExportEnabled(ImportCollection, ImportFilePath.Text);
        }

        private void ExportBrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = ExportFilePath.Text,
                DefaultExt = ".json",
                AddExtension = true,
                OverwritePrompt = true,
                Filter = "JSON Files (*.json)|*.json"
            };
            if (dlg.ShowDialog() == true)
            {
                ExportFilePath.Text = dlg.FileName;
            }
            Export.IsEnabled = IsImportExportEnabled(ExportCollection, ExportFilePath.Text);
        }

        private void ExportCollection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Export.IsEnabled = IsImportExportEnabled(ExportCollection, ExportFilePath.Text);
        }

        private bool IsImportExportEnabled(ComboBox collection, string path)
        {
            return collection.SelectedItem != null && !string.IsNullOrEmpty(path);
        }

        private void ImportExport_Click(object sender, RoutedEventArgs e)
        {
            int switchInt = ImportTab.IsSelected ? ImportCollection.SelectedIndex : ExportCollection.SelectedIndex;
            switch (switchInt)
            {
                case 0:
                    //Import Guests
                    if (ImportTab.IsSelected)
                    {
                    }
                    //Export Guests
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Guests);
                    break;
                case 1:
                    //Import Reservations
                    if (ImportTab.IsSelected) controller.ImportReservations(ImportFilePath.Text);
                    //Export Reservations
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Reservations
                        .Include(r => r.Room.Floor.Building).Include(r => r.Guest));
                    break;
                case 2:
                    //Import Transactions
                    if (ImportTab.IsSelected)
                    {
                    }
                    //Export Transactions
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Transactions
                        .Include(t => t.Guest).Include(t => t.Reservation));
                    break;
                case 3:
                    //Import Buildings
                    if (ImportTab.IsSelected) JsonImport.ImportBuildings(controller, ImportFilePath.Text);
                    //Export Buildings
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Buildings);
                    break;
                case 4:
                    //Import Floors
                    if (ImportTab.IsSelected) JsonImport.ImportFloors(controller, ImportFilePath.Text);
                    //Export Floors
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Floors.Include(f => f.Building));
                    break;
                case 5:
                    //Import Rooms
                    if (ImportTab.IsSelected) controller.ImportRooms(ImportFilePath.Text);
                    //Export Rooms
                    else ExportCollectionsToJson(ExportFilePath.Text, controller.Context.Rooms.Include(r => r.Floor.Building));
                    break;
            }
            Close();
        }

        private void ExportCollectionsToJson(string filePath, IEnumerable<object> collection)
        {
            if (FileHandler.WriteAllLines(filePath, JsonHandler.GetJsonStrings(collection)))
            {
                string messageText = $"Successfully exported {ExportCollection.SelectedItem} to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                MessageBox.Show(messageText, "Export success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string messageText = $"Failed to export {ExportCollection.SelectedItem} to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                MessageBox.Show(messageText, "Export fail!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}