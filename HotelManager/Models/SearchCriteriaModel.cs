using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManager.Models
{
    public class SearchCriteriaModel : INotifyPropertyChanged
    {
        private string text;
        private DateTime? startDate;
        private DateTime? endDate;

        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public SearchCriteriaModel(string text, DateTime? startDate, DateTime? endDate)
        {
            Text = text;
            StartDate = startDate;
            EndDate = endDate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}