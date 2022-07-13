using MaterialDesignThemes.Wpf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using User = OASIS_App.MVVM.Model.User;
using SqlConn = OASIS_App.MVVM.Model.SqlConn;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        protected readonly SqlConnection _sqlConnection = SqlConn.Connection;

        private User _user = new User();

        public LogInPage()
        {
            InitializeComponent();

            DataContext = _user;
        }

        // Authorizes user
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DataIsEneterdCorrectly(_user.Login, PasswordBox.Password))
            {
                return;
            }

            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Login='" + _user.Login + "' AND Password='" + PasswordBox.Password + "'", _sqlConnection);
            SqlDataReader dataReader = command.ExecuteReader();

            try
            {
                if (!dataReader.Read())
                {
                    SnackBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.5));
                    SnackBar.MessageQueue.Enqueue("Неверный логин или пароль");
                    return;
                }

                _user.Id = (int)dataReader.GetValue(0);
                _user.Email = dataReader.GetValue(2).ToString();
                _user.MobilePhone = dataReader.GetValue(3).ToString();

                NavigationService.Content = null;

                MainWindow objMainWindows = (MainWindow)Window.GetWindow(this);
                objMainWindows.Frame.Navigate(new Test());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
            finally
            {
                _sqlConnection.Close();
                dataReader.Close();
            }
        }

        // Checks if TextBoxes info was entered correctly
        private bool DataIsEneterdCorrectly(string login, string password)
        {
            SnackBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.5));

            if (string.IsNullOrEmpty(login))
            {
                SnackBar.MessageQueue.Enqueue("Введите логин");
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                SnackBar.MessageQueue.Enqueue("Введите пароль");
                return false;
            }

            if (ContainsInvalidSymbols(login, "Логин", out string errorMessage))
            {
                SnackBar.MessageQueue.Enqueue(errorMessage);
                return false;
            }

            if (ContainsInvalidSymbols(password, "Пароль", out errorMessage))
            {
                SnackBar.MessageQueue.Enqueue(errorMessage);
                return false;
            }

            return true;

            bool ContainsInvalidSymbols(string str, string fieldName, out string message)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsPunctuation(str[i]))
                    {
                        message = $"Был введён неверный знак {str[i]} в поле {fieldName}";
                        return true;
                    }
                }

                message = null;
                return false;
            }
        }
    }
}
