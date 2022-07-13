using MaterialDesignThemes.Wpf;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Page
    {
        public Test()
        {
            InitializeComponent();
        }

        // Loads pages and shows "welcome" message in different threads
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread loadPages = new Thread(LoadPages);
            Thread welcomeMessage = new Thread(ShowWelcomeMessage);

            loadPages.Start();
            welcomeMessage.Start();
        }

        // Shows "welcome" message
        private void ShowWelcomeMessage()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                SnackBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2.5));
                SnackBar.MessageQueue.Enqueue("Добро пожаловать в OASIS");
            });
        }

        // Loads pages
        private void LoadPages()
        {
            Dispatcher.Invoke(DispatcherPriority.Render, (ThreadStart)delegate
            {
                MainMenuFrame.Navigate(new HomeView());
                AvailableToursFrame.Navigate(new AvailableTours());
                ProfileFrame.Navigate(new UserProfile());
                CreateTour.Navigate(new MakeTour());
            });
        }
    }
}
