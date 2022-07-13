using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using Image = OASIS_App.MVVM.Model.Image;
using Tour = OASIS_App.MVVM.Model.Tour;
using MessageBox = System.Windows.MessageBox;
using SqlConn = OASIS_App.MVVM.Model.SqlConn;
using System.ComponentModel;
using System.Text;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for MakeTour.xaml
    /// </summary>
    public partial class MakeTour : Page, INotifyPropertyChanged
    {
        private SqlConnection _sqlConnection = SqlConn.Connection;

        private Tour _tour = new Tour();

        public MakeTour()
        {
            InitializeComponent();

            DataContext = _tour;

            ClearFields();
        }

        // Saves a Tour
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            if (FieldsEnteredCorrectly())
            {
                try
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "TourImages\\", _tour.Image.Name);
                    FileInfo image = new FileInfo(_tour.Image.Path);
                    image.CopyTo(folder, true);
                    _tour.Image.Path = folder;

                    SqlCommand command = new SqlCommand("INSERT INTO Tours VALUES (@Name, @PriceAdult, @PriceChild, @Duration, @Date, @Info, @ImagePath)", _sqlConnection);
                    command.Parameters.AddWithValue("@Name", Name.Text);
                    command.Parameters.AddWithValue("@PriceAdult", Convert.ToDouble(PriceAdult.Text));
                    command.Parameters.AddWithValue("@PriceChild", Convert.ToDouble(PriceChild.Text));
                    command.Parameters.AddWithValue("@Duration", Convert.ToInt16(Duration.Text));
                    command.Parameters.AddWithValue("@Date", AvailableDate.SelectedDate);
                    command.Parameters.AddWithValue("@Info", Info.Text);
                    command.Parameters.AddWithValue("@ImagePath", _tour.Image.Path);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Операция прошла успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    ClearFields();
                    _sqlConnection.Close();
                }
            }
        }

        // Shows a tour
        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            SqlCommand command = new SqlCommand("SELECT * FROM Tours WHERE Id='" + Id.Text + "'", _sqlConnection);

            SqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                MessageBox.Show("Нет тура с таким Id");
                return;
            }

            try
            {
                while (reader.Read())
                {
                    _tour.Id = reader.GetInt32(0);
                    _tour.Name = reader.GetString(1);
                    _tour.PriceAdult = reader.GetDecimal(2);
                    _tour.PriceChild = reader.GetDecimal(3);
                    _tour.Duration = reader.GetInt32(4);
                    _tour.AvailableDate = reader.GetDateTime(5);
                    _tour.Info = reader.GetString(6);

                    string imagePath = reader.GetValue(7).ToString();

                    _tour.Image = new Image(imagePath, imagePath.Substring(imagePath.LastIndexOf('\\') + 1));
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
        }

        // Updates SQL record
        private void UpdateTour_Click(object sender, RoutedEventArgs e)
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            if (FieldsEnteredCorrectly())
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "TourImages\\", _tour.Image.Name);
                FileInfo image = new FileInfo(_tour.Image.Path);

                if (!image.Exists)
                {
                    image.CopyTo(folder, true);
                    _tour.Image.Path = folder;
                }

                SqlCommand command = new SqlCommand("UPDATE Tours SET Name=@Name, " +
                                                    "PriceAdult=@PriceAdult, " +
                                                    "PriceChild=@PriceChild, " +
                                                    "Duration=@Duration, " +
                                                    "Date=@Date, " +
                                                    "Info=@Info, " +
                                                    "ImagePath=@ImagePath " +
                                                    "WHERE Id=@Id", _sqlConnection);

                command.Parameters.AddWithValue("@Id", Id.Text);
                command.Parameters.AddWithValue("@Name", _tour.Name);
                command.Parameters.AddWithValue("@PriceAdult", _tour.PriceAdult);
                command.Parameters.AddWithValue("@PriceChild", _tour.PriceChild);
                command.Parameters.AddWithValue("@Duration", _tour.Duration);
                command.Parameters.AddWithValue("@Date", AvailableDate.SelectedDate);
                command.Parameters.AddWithValue("@Info", _tour.Info);
                command.Parameters.AddWithValue("@ImagePath", _tour.Image.Path);
                command.ExecuteNonQuery();

                MessageBox.Show("Операция прошла успешно");

                _sqlConnection.Close();
            }
        }

        // Opens File Dialogue to browse an image
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialogue = new OpenFileDialog();

                dialogue.Filter = "JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
                dialogue.Title = "Select Picture";

                if (dialogue.ShowDialog() == DialogResult.OK)
                {
                    TourImage.Source = new BitmapImage(new Uri(dialogue.FileName));

                    Image image = new Image();

                    image.Path = dialogue.FileName;
                    image.Name = dialogue.FileName.Substring(dialogue.FileName.LastIndexOf('\\') + 1).ToLower();

                    _tour.Image = image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Opens SQL Connection
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_sqlConnection.State is ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }
        }

        // Clears Fields
        private void ClearFields()
        {
            Name.Text = null;
            PriceAdult.Text = null;
            PriceChild.Text = null;
            Duration.Text = null;
            Info.Text = null;
            AvailableDate.Text = null;
            TourImage.Source = null;
        }

        // TODO Checks if all fiels entered correctly
        private bool FieldsEnteredCorrectly()
        {
            if (TourImage.Source is null)
            {
                MessageBox.Show("Необходимо выбрать обложку тура");
                return false;
            }

            if (string.IsNullOrEmpty(Name.Text))
            {
                MessageBox.Show("Название тура не может быть пустым");
                return false;
            }

            if (ContainsInvalidSymbols(Name.Text) || ContainsNumbers(Name.Text))
            {
                MessageBox.Show("Название тура содержит неверные символы");
                Name.Text = string.Empty;

                return false;
            }

            if (ContainsLetters(Duration.Text))
            {
                MessageBox.Show("Длительность тура не может содержать буквы");
                Duration.Text = string.Empty;

                return false;
            }

            if (int.Parse(Duration.Text) <= 0)
            {
                MessageBox.Show("Длительность тура не может быть меньше либо равна 0");
                Duration.Text = string.Empty;

                return false;
            }

            if (ContainsLetters(PriceAdult.Text))
            {
                MessageBox.Show("Стоимость тура для взрослого не может содержать буквы");
                PriceAdult.Text = string.Empty;

                return false;
            }

            if (ContainsLetters(PriceChild.Text))
            {
                MessageBox.Show("Стоимость тура для ребёнка не может содержать буквы");
                PriceChild.Text = string.Empty;

                return false;
            }


            if (!AvailableDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Необходимо указать дату тура");
                return false;
            }

            if (AvailableDate.SelectedDate.Value < DateTime.Now)
            {
                MessageBox.Show("Неверно выбрана дата");
                AvailableDate.Text = string.Empty;

                return false;
            }

            if (string.IsNullOrEmpty(Info.Text))
            {
                MessageBox.Show("Необходимо указать информацию о туре");
                return false;
            }

            return true;

            bool ContainsInvalidSymbols(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentNullException(nameof(text));
                }

                StringBuilder stringBuilder = new StringBuilder(text);

                for (int i = 0; i < stringBuilder.Length; i++)
                {
                    if (char.IsPunctuation(stringBuilder[i]) && stringBuilder[i] != '-')
                    {
                        return true;
                    }

                    if (stringBuilder[i] >= 97 && stringBuilder[i] <= 122)
                    {
                        return true;
                    }
                }

                return false;
            }

            bool ContainsNumbers(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentNullException(nameof(text));
                }

                StringBuilder stringBuilder = new StringBuilder(text);

                for (int i = 0; i < stringBuilder.Length; i++)
                {
                    if (char.IsNumber(stringBuilder[i]))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool ContainsLetters(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    throw new ArgumentNullException(nameof(text));
                }

                StringBuilder stringBuilder = new StringBuilder(text);

                for (int i = 0; i < stringBuilder.Length; i++)
                {
                    if (char.IsLetter(stringBuilder[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
