using OASIS_App.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tour = OASIS_App.MVVM.Model.Tour;
using System.ComponentModel;

namespace OASIS_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for AvailableTours.xaml
    /// </summary>
    public partial class AvailableTours : Page, INotifyPropertyChanged
    {
        protected List<Tour> _availableTours;

        public AvailableTours()
        {
            InitializeComponent();
        }

        // Fills Available Tours' Frame with Available for booking (tours, that user hasn't already booked) tours
        public void FillFrameWithTours(List<Tour> tours)
        {
            if (tours.Count == 0)
            {
                throw new ArgumentNullException(nameof(tours));
            }

            if (ToursWrapPanel.Children.Count != 0)
            {
                ToursWrapPanel.Children.Clear();
            }

            for (int i = 0; i < tours.Count; i++)
            {
                ToursWrapPanel.Children.Add(new TourCard(tours[i]));
            }
        }

        // Refreshes Tours Frame
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _availableTours = User.GetAvailableTours();

            FillFrameWithTours(_availableTours);
        }

        // Clears Tours Frame
        private void ClearFilledComboBoxButton_Click(object sender, RoutedEventArgs e)
        {
            FilledComboBox.SelectedIndex = -1;

            FillFrameWithTours(_availableTours);
        }

        // Sorts Tours
        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FilledComboBox.SelectedIndex)
            {
                case 0:
                    FillFrameWithTours(_availableTours.OrderBy(a => a.PriceAdult).ToList());
                    break;
                case 1:
                    FillFrameWithTours(_availableTours.OrderByDescending(a => a.PriceAdult).ToList());
                    break;
                case 2:
                    FillFrameWithTours(_availableTours.OrderBy(a => a.Name).ToList());
                    break;
                case 3:
                    FillFrameWithTours(_availableTours.OrderBy(a => a.AvailableDate).ToList());
                    break;
                case 4:
                    FillFrameWithTours(_availableTours.OrderByDescending(a => a.AvailableDate).ToList());
                    break;
                default:
                    return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
