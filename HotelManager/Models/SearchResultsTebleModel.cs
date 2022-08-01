using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManager.Models
{
    public class SearchResultsTebleModel : INotifyPropertyChanged
    {
        private ObservableCollection<SearchResultModel> results;

        public ObservableCollection<SearchResultModel> Results
        {
            get => results;
            set
            {
                if (results != value)
                {
                    results = value;
                    OnPropertyChanged();
                }
            }
        }

        public SearchResultsTebleModel()
        {
            Results = new ObservableCollection<SearchResultModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}