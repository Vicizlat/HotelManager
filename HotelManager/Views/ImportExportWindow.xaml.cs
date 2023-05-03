using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
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
            if (ImportTab.IsSelected)
            {
                switch (ImportCollection.SelectedIndex)
                {
                    case 0:
                        controller.ImportBuildings(ImportFilePath.Text);
                        break;
                    case 1:
                        controller.ImportFloors(ImportFilePath.Text);
                        break;
                    case 2:
                        controller.ImportRooms(ImportFilePath.Text);
                        break;
                    case 3:
                        controller.ImportGuests(ImportFilePath.Text);
                        break;
                    case 4:
                        controller.ImportReservations(ImportFilePath.Text);
                        break;
                    case 5:
                        controller.ImportTransactions(ImportFilePath.Text);
                        break;
                    case 6:
                        controller.ImportPriceRanges(ImportFilePath.Text);
                        break;
                }
            }
            else
            {
                string selectedItemName = ExportCollection.SelectedItem.ToString();
                controller.ExportCollectionToJson(ExportFilePath.Text, controller.GetCollectionByName(selectedItemName), selectedItemName);
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}