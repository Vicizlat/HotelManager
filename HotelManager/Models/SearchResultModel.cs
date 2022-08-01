using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HotelManager.Models
{
    public class SearchResultModel : INotifyPropertyChanged
    {
        private string id;
        private string state;
        private string source;
        private string room;
        private string guest;
        private string dates;
        private string sums;
        private string notes;
        private string tooltip;

        public ICommand TextBoxDoubleClickCommand { get; private set; }
        public string Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }
        public string State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Source
        {
            get => source;
            set
            {
                if (source != value)
                {
                    source = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Room
        {
            get => room;
            set
            {
                if (room != value)
                {
                    room = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Guest
        {
            get => guest;
            set
            {
                if (guest != value)
                {
                    guest = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Dates
        {
            get => dates;
            set
            {
                if (dates != value)
                {
                    dates = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Sums
        {
            get => sums;
            set
            {
                if (sums != value)
                {
                    sums = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Notes
        {
            get => notes;
            set
            {
                if (notes != value)
                {
                    notes = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Tooltip
        {
            get => tooltip;
            set
            {
                if (tooltip != value)
                {
                    tooltip = value;
                    OnPropertyChanged();
                }
            }
        }

        public SearchResultModel(string[] result, string tooltip, ICommand command)
        {
            Id = result[0];
            State = result[1];
            Source = result[2];
            Room = result[3];
            Guest = result[4];
            Dates = result[5];
            Sums = result[6];
            Notes = result[7];
            Tooltip = tooltip;
            TextBoxDoubleClickCommand = command;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}