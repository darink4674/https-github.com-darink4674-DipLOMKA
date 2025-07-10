
using DIPLOM.Connection;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class FlightSearch : Page
    {
        private int currentUserId;

        public FlightSearch(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadAirports();
            FlightDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadAirports()
        {
            try
            {
                DepartureComboBox.ItemsSource = DB.air.Airports.ToList();
                ArrivalComboBox.ItemsSource = DB.air.Airports.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки аэропортов: " + ex.Message);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DepartureComboBox.SelectedItem == null ||
                    ArrivalComboBox.SelectedItem == null ||
                    FlightDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля для поиска");
                    return;
                }

                var departure = (Airports)DepartureComboBox.SelectedItem;
                var arrival = (Airports)ArrivalComboBox.SelectedItem;
                var date = FlightDatePicker.SelectedDate.Value.Date;

                var flights = (from f in DB.air.Flights
                               join dep in DB.air.Airports on f.DepartureAirportId equals dep.AirportId
                               join arr in DB.air.Airports on f.ArrivalAirportId equals arr.AirportId
                               join ac in DB.air.Aircrafts on f.AircraftId equals ac.AircraftId
                               where f.DepartureAirportId == departure.AirportId
                               && f.ArrivalAirportId == arrival.AirportId
                               && f.DepartureTime.Year == date.Year
                               && f.DepartureTime.Month == date.Month
                               && f.DepartureTime.Day == date.Day
                               && f.IsActive
                               select new
                               {
                                   f.FlightId,
                                   f.FlightNumber,
                                   DepartureAirport = dep,
                                   ArrivalAirport = arr,
                                   f.DepartureTime,
                                   f.ArrivalTime,
                                   Aircraft = ac,
                                   f.BasePrice
                               }).ToList();

                FlightsListView.ItemsSource = flights;

                if (flights.Count == 0)
                {
                    MessageBox.Show("Рейсы по заданным параметрам не найдены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске рейсов: " + ex.Message);
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите рейс из списка");
                return;
            }

            dynamic selectedItem = FlightsListView.SelectedItem;
            var flight = new Flights
            {
                FlightId = selectedItem.FlightId,
                FlightNumber = selectedItem.FlightNumber,
                DepartureAirportId = selectedItem.DepartureAirport.AirportId,
                ArrivalAirportId = selectedItem.ArrivalAirport.AirportId,
                DepartureTime = selectedItem.DepartureTime,
                ArrivalTime = selectedItem.ArrivalTime,
                AircraftId = selectedItem.Aircraft.AircraftId,
                BasePrice = selectedItem.BasePrice
            };

            NavigationService.Navigate(new BookingForm(flight, currentUserId));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new CustomerMenu(currentUserId));
            }
        }
    }
}