using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;

namespace OASIS_App.MVVM.Model
{
    public class Tour : INotifyPropertyChanged
    {
        protected static readonly SqlConnection _sqlConnection = SqlConn.Connection;

        private int _ordersAmount;
        private string _name;
        private string _info;
        private int _id;
        private int _duration;
        private decimal _priceAdult;
        private decimal _priceChild;
        private DateTime _availableDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private Image _image;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public int Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public decimal PriceAdult
        {
            get => Math.Round(_priceAdult, 2);
            set
            {
                _priceAdult = value;
                OnPropertyChanged("PriceAdult");
            }
        }

        public decimal PriceChild
        {
            get => _priceChild == 0 ? PriceAdult : Math.Round(_priceChild, 2);
            set
            {
                _priceChild = value;
                OnPropertyChanged("PriceChild");
            }
        }

        public DateTime AvailableDate
        {
            get => _availableDate;
            set
            {
                _availableDate = value;
                OnPropertyChanged("AvailableDate");
            }
        }

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        public int OrdersAmount
        {
            get => _ordersAmount;
            set
            {
                _ordersAmount = value;
                OnPropertyChanged("OrdersAmount");
            }
        }

        public Tour() { }

        // Gets All tours
        public static List<Tour> GetAllTours()
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            List<Tour> tours = new List<Tour>();

            SqlCommand command = new SqlCommand("SELECT * FROM Tours", _sqlConnection);

            SqlDataReader dataReader = command.ExecuteReader();

            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        Tour tour = new Tour();

                        tour.Id = dataReader.GetInt32(0);
                        tour.Name = dataReader.GetString(1);
                        tour.PriceAdult = dataReader.GetDecimal(2);
                        tour.PriceChild = dataReader.GetDecimal(3);
                        tour.Duration = dataReader.GetInt32(4);
                        tour.AvailableDate = dataReader.GetDateTime(5);
                        tour.Info = dataReader.GetString(6);
                        tour.Image = new Image(dataReader.GetString(7));

                        tours.Add(tour);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить запись.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _sqlConnection.Close();

                dataReader.Close();
            }

            return tours;
        }

        // Gets Cheapest tour
        public static Tour GetCheapest()
        {
            List<Tour> tours = GetAllTours();

            tours = tours.OrderBy(x => x.PriceAdult).ToList();

            return tours[0];
        }

        // Gets Upcoming tour
        public static Tour GetUpcoming()
        {
            List<Tour> tours = GetAllTours();

            tours = tours.OrderBy(x => x.AvailableDate).ToList();

            return tours[0];
        }

        // Gets The most ordered tour and amount of orders
        public static Tour GetMostOrdered()
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            Tour mostOrderedTour = new Tour();

            SqlCommand command = new SqlCommand("SELECT TourId, COUNT(*) AS CNT FROM BookedTrips GROUP BY TourId ORDER BY CNT DESC", _sqlConnection);
            SqlDataReader dataReader = command.ExecuteReader();

            try
            {
                dataReader.Read();

                int tourId = dataReader.GetInt32(0);
                mostOrderedTour.OrdersAmount = dataReader.GetInt32(1);

                dataReader.Close();

                command = new SqlCommand("SELECT Name FROM Tours WHERE Id=@Id", _sqlConnection);
                command.Parameters.AddWithValue("Id", tourId);

                dataReader = command.ExecuteReader();

                dataReader.Read();

                mostOrderedTour.Name = dataReader.GetString(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _sqlConnection.Close();
                dataReader.Close();
            }

            return mostOrderedTour;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
