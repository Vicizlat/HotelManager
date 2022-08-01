using System;
using System.Windows.Input;

namespace HotelManager.ViewModels.Commands
{
    public class SearchButtonCommand : ICommand
    {
        private readonly SearchViewModel viewModel;

        public SearchButtonCommand(SearchViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (viewModel.SearchOptionsModel.SearchStartDate || viewModel.SearchOptionsModel.SearchEndDate)
            {
                return viewModel.SearchCriteriaModel.StartDate.HasValue && viewModel.SearchCriteriaModel.EndDate.HasValue;
            }
            return true;
        }

        public void Execute(object parameter) => viewModel.SearchButtonPressed();
    }
}