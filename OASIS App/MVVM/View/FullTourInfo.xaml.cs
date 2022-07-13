using System.Windows.Controls;
using Tour = OASIS_App.MVVM.Model.Tour;
using User = OASIS_App.MVVM.Model.User;
using AvailableTours = OASIS_App.MVVM.View.AvailableTours;
using SqlConn = OASIS_App.MVVM.Model.SqlConn;
using System.Windows.Navigation;
using System.Data.SqlClient;
using System.Windows;
using System;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Windows.Threading;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for FullTourInfo.xaml
    /// </summary>
    public partial class FullTourInfo : UserControl
    {
        protected SqlConnection _sqlConnection = SqlConn.Connection;

        protected User _user = new User();

        private Tour _currentTour;
        
        // class constructor
        public FullTourInfo(Tour tour)
        {
            InitializeComponent();

            _currentTour = tour;

            DataContext = _currentTour;
        }

        // closes the window
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow objMainWindows = (MainWindow)Window.GetWindow(this);
            objMainWindows.MainFrame.Navigate(null);
        }

        // Puts an ordered tour into the database
        private void OrderTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO [BookedTrips] (UserId, TourId) VALUES (@UserId, @TourId)", _sqlConnection);
                command.Parameters.AddWithValue("UserId", _user.Id);
                command.Parameters.AddWithValue("TourId", _currentTour.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _sqlConnection.Close();

                MainWindow objMainWindows = (MainWindow)Window.GetWindow(this);
                objMainWindows.MainFrame.Navigate(null);
            }
        }
    }
}
