using HotelManager.Controller;
using HotelManager.Models;
using HotelManager.ViewModels.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManager.ViewModels
{
    public class SearchViewModel
    {
        public MainController controller;
        public SearchOptionsModel SearchOptionsModel { get; set; }
        public SearchCriteriaModel SearchCriteriaModel { get; set; }
        public SearchStatusBarModel SearchStatusBarModel { get; set; }
        public SearchResultsTebleModel SearchResultsTebleModel { get; set; }
        public ICommand SearchButtonCommand { get; private set; }

        public SearchViewModel(MainController controller)
        {
            this.controller = controller;
            SearchOptionsModel = new SearchOptionsModel(false, false, true);
            SearchCriteriaModel = new SearchCriteriaModel("", null, null);
            SearchStatusBarModel = new SearchStatusBarModel(0, 0, 100, "");
            SearchResultsTebleModel = new SearchResultsTebleModel();
            SearchButtonCommand = new SearchButtonCommand(this);
            controller.OnReservationEdit += delegate { SearchButtonPressed(); };
        }

        public async void SearchButtonPressed()
        {
            controller.ShowWaitWindow("Обработвам търсенето...");
            SearchResultsTebleModel.Results.Clear();
            SearchStatusBarModel.Text = "Търсене...";
            SearchStatusBarModel.Sum = 0;
            await Task.Run(async () => { await SearchResultsAsync(); });
            SearchStatusBarModel.Text = "Търсенето завърши";
            controller.CloseWaitWindow();
        }

        private async Task SearchResultsAsync()
        {
            List<ReservationInfo> reservations = await controller.GetAllReservationInfos(SearchOptionsModel.IncludeCanceled);
            List<ReservationInfo> searchResults = reservations
                .Where(r => r.ToString().ToLower().Contains(SearchCriteriaModel.Text.ToLower()))
                .Where(r => !SearchOptionsModel.SearchStartDate || r.StartDate >= SearchCriteriaModel.StartDate)
                .Where(r => !SearchOptionsModel.SearchStartDate || r.StartDate <= SearchCriteriaModel.EndDate)
                .Where(r => !SearchOptionsModel.SearchEndDate || r.EndDate >= SearchCriteriaModel.StartDate)
                .Where(r => !SearchOptionsModel.SearchEndDate || r.EndDate <= SearchCriteriaModel.EndDate)
                .OrderBy(r => r.Id).ToList();
            SearchStatusBarModel.ProgBarMax = searchResults.Count;
            for (int i = 0; i < searchResults.Count; i++)
            {
                Thread.Sleep(1);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    string[] result = searchResults[i].ToSearchArray();
                    string tooltip = $"{searchResults[i]}";
                    TextBoxDoubleClickCommand command = new TextBoxDoubleClickCommand(controller, searchResults[i].Id);
                    SearchResultsTebleModel.Results.Add(new SearchResultModel(result, tooltip, command));
                    SearchStatusBarModel.Count = i + 1;
                    SearchStatusBarModel.Sum += searchResults[i].TotalSum;
                });
            }
        }
    }
}