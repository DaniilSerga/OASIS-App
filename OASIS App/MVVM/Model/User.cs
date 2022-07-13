using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace OASIS_App.MVVM.Model
{
    public class User : Person
    {
        protected static readonly SqlConnection _sqlConnection = SqlConn.Connection;

        public override int Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        public override string Login
        {
            get => _login;
            set
            {
                _login = value;
            }
        }

        public override string Email
        {
            get => _email;
            set
            {
                _email = value;
            }
        }

        public override string MobilePhone
        {
            get => _mobilePhone;
            set
            {
                _mobilePhone = value;
            }
        }

        public override string Password
        {
            get => _password;
            set
            {
                _password = value;
            }
        }

        // These propperties have 0 references due to their binding to UI fields
        public static int BookedToursAmount => GetBookedTours().Count;

        public static decimal BookedToursTotalPrice
        {
            get
            {
                List<Tour> tours = GetBookedTours();
                decimal totalPrice = 0;

                for (int i = 0; i < tours.Count; i++)
                {
                    totalPrice += tours[i].PriceAdult;
                }

                return totalPrice;
            }
        }

        // Gets all tours user has booked
        public static List<Tour> GetBookedTours()
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            List<Tour> bookedTours = new List<Tour>();

            SqlCommand command = new SqlCommand("SELECT TourId FROM [BookedTrips] WHERE UserId=@UserID", _sqlConnection);
            command.Parameters.AddWithValue("UserId", _id);

            SqlDataReader dataReader = command.ExecuteReader();

            try
            {
                List<int> tourId = new List<int>();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        tourId.Add(dataReader.GetInt32(0));
                    }
                }

                for (int i = 0; i < tourId.Count; i++)
                {
                    dataReader.Close();

                    command = new SqlCommand("SELECT * FROM [Tours] WHERE Id=@Id", _sqlConnection);
                    command.Parameters.AddWithValue("Id", tourId[i]);

                    dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Tour tour = new Tour();

                            tour.Id = (int)dataReader.GetValue(0);
                            tour.Name = dataReader.GetString(1);
                            tour.PriceAdult = dataReader.GetDecimal(2);
                            tour.PriceChild = dataReader.GetDecimal(3);
                            tour.Duration = dataReader.GetInt32(4);
                            tour.AvailableDate = dataReader.GetDateTime(5);
                            tour.Info = dataReader.GetString(6);
                            tour.Image = new Image(dataReader.GetString(7));

                            bookedTours.Add(tour);
                        }
                    }
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

            return bookedTours;
        }

        // Gets user's available Tour
        public static List<Tour> GetAvailableTours()
        {
            return SelectAvailableTours(GetCommandForSelecting());

            string GetCommandForSelecting()
            {
                if (_sqlConnection.State is ConnectionState.Closed)
                {
                    _sqlConnection.Open();
                }

                User user = new User();

                SqlCommand command = new SqlCommand("SELECT TourId FROM BookedTrips WHERE UserId=@UserId", _sqlConnection);
                command.Parameters.AddWithValue("UserId", user.Id);

                SqlDataReader dataReader = command.ExecuteReader();

                List<string> TourIds = new List<string>();

                try
                {
                    while (dataReader.Read())
                    {
                        TourIds.Add($"ID <> '{dataReader.GetInt32(0)}'");
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

                return string.Join(" AND ", TourIds.ToArray());
            }

            List<Tour> SelectAvailableTours(string cmd)
            {
                if (string.IsNullOrEmpty(cmd))
                {
                    return Tour.GetAllTours();
                }

                if (_sqlConnection.State is ConnectionState.Closed)
                {
                    _sqlConnection.Open();
                }

                SqlCommand command = new SqlCommand($"SELECT * FROM Tours WHERE {cmd}", _sqlConnection);

                SqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    MessageBox.Show("Не удалось получить доступные туры");
                }

                List<Tour> result = new List<Tour>();

                try
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

                        result.Add(tour);
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

                if (result.Count == 0)
                {
                    MessageBox.Show("Кол-во доступных туров = 0");
                }
                return result;
            }
        }
    }
}
