using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using Tour = OASIS_App.MVVM.Model.Tour;
using User = OASIS_App.MVVM.Model.User;
using System;
using System.Windows;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Page
    {
        protected List<Tour> _bookedTours;

        protected User _user = new User();

        public UserProfile()
        {
            InitializeComponent();

            DataContext = _user;
        }

        // Fills BookedTours WrapPanel with booked tours
        public void FillWrapPanelWithTours(List<Tour> tours)
        {
            if (tours.Count == 0)
            {
                throw new ArgumentNullException(nameof(tours));
            }

            if (BookedToursWrapPanel.Children.Count != 0)
            {
                BookedToursWrapPanel.Children.Clear();
            }

            for (int i = 0; i < tours.Count; i++)
            {
                BookedToursWrapPanel.Children.Add(new BookedTour(tours[i]));
            }
        }

        // Calls FillWrapPanelWithTours method
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _bookedTours = User.GetBookedTours();

            FillWrapPanelWithTours(_bookedTours);
        }

        // Sorts booked tours
        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FilledComboBox.SelectedIndex)
            {
                case 0:
                    FillWrapPanelWithTours(_bookedTours.OrderBy(a => a.PriceAdult).ToList());
                    break;
                case 1:
                    FillWrapPanelWithTours(_bookedTours.OrderByDescending(a => a.PriceAdult).ToList());
                    break;
                case 2:
                    FillWrapPanelWithTours(_bookedTours.OrderBy(a => a.Name).ToList());
                    break;
                case 3:
                    FillWrapPanelWithTours(_bookedTours.OrderBy(a => a.AvailableDate).ToList());
                    break;
                case 4:
                    FillWrapPanelWithTours(_bookedTours.OrderByDescending(a => a.AvailableDate).ToList());
                    break;
                default:
                    return;
            }
        }

        // Clears BookedTours WrapPanel
        private void ClearFilledComboBoxButton_Click(object sender, RoutedEventArgs e)
        {
            BookedToursWrapPanel.Children.Clear();
        }
    }
}
