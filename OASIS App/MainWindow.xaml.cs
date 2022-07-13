using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace OASIS_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Shows start registration page
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MVVM.View.RegPage());
        }

        // Allows to minimize window's size
        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        // Allows to move the window while it is being dragged
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton is MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Logic for ShutDown button
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
