using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using User = OASIS_App.MVVM.Model.User;
using SqlConn = OASIS_App.MVVM.Model.SqlConn;
using System.Windows.Media.Animation;
using System;
using System.Data;
using System.Threading;
using MaterialDesignThemes.Wpf;
using System.Linq;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        protected readonly SqlConnection _sqlConnection = SqlConn.Connection;

        private User _user = new User();

        public RegPage()
        {
            InitializeComponent();

            DataContext = _user;

            StartMenuAnimation();
        }

        // Makes a new SQL query to Add a new user
        public void RegButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UserAlreadyExists() && TextBoxesEnteredCorrectly())
            {
                Thread userRegistration = new Thread(UserRegistration);
                userRegistration.Start();

                NavigationService.Content = null;

                MainWindow objMainWindows = (MainWindow)Window.GetWindow(this);
                objMainWindows.Frame.Navigate(new Test());
            }
        }

        // Checks if all the checkBoxes are entered in correct format
        protected bool TextBoxesEnteredCorrectly()
        {
            if (_user.Login.Length == 0)
            {
                ShowErrorMessage("Укажите логин");
                return false;
            }

            if (_user.Email.Length == 0)
            {
                ShowErrorMessage("Укажите email");
                return false;
            }

            if (_user.MobilePhone.Length == 0)
            {
                ShowErrorMessage("Укажите номер телефона");
                return false;
            }

            if (PasswordBox.Password.Length == 0)
            {
                ShowErrorMessage("Укажите пароль");
                return false;
            }

            return true;
        }

        // Adds new sql record
        protected void UserRegistration()
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            try
            {
                SqlCommand command = new SqlCommand("INSERT INTO [Users] (Login, Email, MobilePhone, Password) VALUES (@Login, @Email, @MobilePhone, @Password)", _sqlConnection);

                command.Parameters.AddWithValue("Login", _user.Login);
                command.Parameters.AddWithValue("Email", _user.Email);
                command.Parameters.AddWithValue("MobilePhone", _user.MobilePhone);
                command.Parameters.AddWithValue("Password", PasswordBox.Password);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        // Checks if user alrady exists in database
        protected bool UserAlreadyExists()
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            SqlCommand command = new SqlCommand("SELECT Login, Email, MobilePhone FROM [Users] WHERE (Login=@Login OR Email=@Email OR MobilePhone=@MobilePhone)", _sqlConnection);
            command.Parameters.AddWithValue("Login", _user.Login);
            command.Parameters.AddWithValue("Email", _user.Email);
            command.Parameters.AddWithValue("MobilePhone", _user.MobilePhone);

            SqlDataReader reader = command.ExecuteReader();

            try
            {
                reader.Read();

                if (reader.HasRows)
                {
                    if (reader.GetString(0) == _user.Login)
                    {
                        ShowErrorMessage($"Логин {_user.Login} уже занят");
                    }
                    else if (reader.GetString(1) == _user.Email)
                    {
                        ShowErrorMessage($"Адрес {_user.Email} уже занят");
                    }
                    else if (reader.GetString(2) == _user.MobilePhone)
                    {
                        ShowErrorMessage($"Номер {_user.MobilePhone} уже занят");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _sqlConnection.Close();

                reader.Close();
            }

            return false;
        }

        // Increases Page's opacity from 0 to 1
        private void StartMenuAnimation()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(1.8);
            MainBorder.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        // Displays error message on a screen
        private void ShowErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            SnackBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.5));
            SnackBar.MessageQueue.Enqueue(message);
        }

        // Checks if any incorrect syntax is occured
        private void MobilePhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = new string
                    (
                         textBox.Text.Where (ch => ch == '+' || (ch >= '0' && ch <= '9')).ToArray()
                    );
            }
        }
    }
}
