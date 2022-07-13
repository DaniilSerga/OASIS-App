using System.Windows;
using System.Windows.Controls;
using Tour = OASIS_App.MVVM.Model.Tour;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private Tour _cheapestTour;
        private Tour _upcomingTour;
        private Tour _mostOrderedTour;

        public HomeView()
        {
            InitializeComponent();
        }

        // Fills Cards
        private void FillAllCards()
        {
            CheapestName.DataContext = _cheapestTour;
            CheapestPrice.DataContext = _cheapestTour;

            UpcomingName.DataContext = _upcomingTour;
            UpcomingDate.DataContext = _upcomingTour;

            MostOrderedName.DataContext = _mostOrderedTour;
            MostOrderedAmount.DataContext = _mostOrderedTour;
        }

        // Updates Cards' info
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _cheapestTour = Tour.GetCheapest();
            _upcomingTour = Tour.GetUpcoming();
            _mostOrderedTour = Tour.GetMostOrdered();

            FillAllCards();
        }
    }
}
