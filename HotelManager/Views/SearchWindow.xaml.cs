using HotelManager.Controller;
using HotelManager.ViewModels;

namespace HotelManager.Views
{
    public partial class SearchWindow
    {
        public SearchWindow(MainController controller)
        {
            InitializeComponent();
            DataContext = new SearchViewModel(controller);
        }
    }
}