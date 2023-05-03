using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotelManager.Models
{
    public class SearchStatusBarModel : INotifyPropertyChanged
    {
        private int count;
        private decimal sum;
        private int nights;
        private double progBarMax;
        private string text;

        public int Count
        {
            get => count;
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal Sum
        {
            get => sum;
            set
            {
                if (sum != value)
                {
                    sum = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Nights
        {
            get => nights;
            set
            {
                if (nights != value)
                {
                    nights = value;
                    OnPropertyChanged();
                }
            }
        }
        public double ProgBarMax
        {
            get => progBarMax;
            set
            {
                if (progBarMax != value)
                {
                    progBarMax = value;
                    OnPropertyChanged();
                }
            }
        }
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

        public SearchStatusBarModel(int count, decimal sum, int nights, double progBarMax, string text)
        {
            Count = count;
            Sum = sum;
            Nights = nights;
            ProgBarMax = progBarMax;
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}