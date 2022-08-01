using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManager.Models
{
    public class SearchOptionsModel : INotifyPropertyChanged
    {
        private bool searchStartDate;
        private bool searchEndDate;
        private bool includeCanceled;

        public bool SearchStartDate
        {
            get => searchStartDate;
            set
            {
                if (searchStartDate != value)
                {
                    searchStartDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool SearchEndDate
        {
            get => searchEndDate;
            set
            {
                if (searchEndDate != value)
                {
                    searchEndDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IncludeCanceled
        {
            get => includeCanceled;
            set
            {
                if (includeCanceled != value)
                {
                    includeCanceled = value;
                    OnPropertyChanged();
                }
            }
        }

        public SearchOptionsModel(bool searchStartDate, bool searchEndDate, bool includeCanceled)
        {
            SearchStartDate = searchStartDate;
            SearchEndDate = searchEndDate;
            IncludeCanceled = includeCanceled;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}