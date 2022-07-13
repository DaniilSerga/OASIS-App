using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using Tour = OASIS_App.MVVM.Model.Tour;
using User = OASIS_App.MVVM.Model.User;
using SqlConn = OASIS_App.MVVM.Model.SqlConn;
using MessageBox = System.Windows.MessageBox;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for BookedTour.xaml
    /// </summary>
    public partial class BookedTour : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private Tour _tour;

        public Tour Tour
        {
            get => _tour;
            set
            {
                _tour = value;
                OnPropertyChanged("BookedTour");
            }
        }

        public BookedTour(Tour tour)
        {
            if (tour is null)
            {
                throw new ArgumentNullException(nameof(tour));
            }

            InitializeComponent();

            _tour = tour;

            DataContext = Tour;
        }

        public BookedTour() { }

        // Deletes Booked Tour
        private void DeleteOrderedTour_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Вы уверены что хотите удалить тур?", "Подтввердите действие", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.No)
            {
                return;
            }

            User user = new User();

            SqlConnection sqlConnection = SqlConn.Connection;
            if (sqlConnection.State is ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            try
            {
                SqlCommand command = new SqlCommand("DELETE FROM BookedTrips WHERE UserId=@UserId AND TourId=@TourId", sqlConnection);
                command.Parameters.AddWithValue("UserId", user.Id);
                command.Parameters.AddWithValue("TourId", _tour.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
