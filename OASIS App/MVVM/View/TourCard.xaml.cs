using System.Windows.Controls;
using Tour = OASIS_App.MVVM.Model.Tour;
using System.Windows;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for TourCard.xaml
    /// </summary>
    public partial class TourCard : UserControl
    {
        private Tour _currentTour;

        public TourCard(Tour tour)
        {
            InitializeComponent();

            _currentTour = tour;

            DataContext = _currentTour;
        }

        // Opens FullTourInfo page
        private void AboutTourButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow objMainWindows = (MainWindow)Window.GetWindow(this);
            objMainWindows.MainFrame.Navigate(new FullTourInfo(_currentTour));
        }
    }
}
