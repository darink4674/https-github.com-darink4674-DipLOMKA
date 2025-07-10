using DIPLOM.Connection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class BookingConfirmation : Page
    {
        private readonly Bookings _booking;
        private readonly int _currentUserId;

        public BookingConfirmation(Bookings booking, int userId)
        {
            if (booking == null)
            {
                MessageBox.Show("Ошибка: данные бронирования не получены");
                return;
            }

            InitializeComponent();
            _booking = booking;
            _currentUserId = userId;
            DisplayBookingDetails();
        }

        private void DisplayBookingDetails()
        {
            try
            {
                FlightDetailsPanel.Children.Clear();
                PassengerDetailsPanel.Children.Clear();
                ServicesPanel.Children.Clear();

                var flight = DB.air.Flights.Find(_booking.FlightId);
                if (flight == null)
                {
                    MessageBox.Show("Информация о рейсе не найдена");
                    return;
                }

                var departure = DB.air.Airports.Find(flight.DepartureAirportId);
                var arrival = DB.air.Airports.Find(flight.ArrivalAirportId);
                var aircraft = DB.air.Aircrafts.Find(flight.AircraftId);

                // Добавляем информацию о рейсе
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Рейс: {flight.FlightNumber}" });
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Откуда: {departure?.City} ({departure?.AirportCode})" });
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Куда: {arrival?.City} ({arrival?.AirportCode})" });
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Вылет: {flight.DepartureTime:g}" });
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Прибытие: {flight.ArrivalTime:g}" });
                FlightDetailsPanel.Children.Add(new TextBlock { Text = $"Самолет: {aircraft?.Model}" });

                // Информация о пассажире
                var passenger = DB.air.Passengers.FirstOrDefault(p => p.BookingId == _booking.BookingId);
                if (passenger != null)
                {
                    PassengerDetailsPanel.Children.Add(new TextBlock { Text = $"Имя: {passenger.FirstName}" });
                    PassengerDetailsPanel.Children.Add(new TextBlock { Text = $"Фамилия: {passenger.LastName}" });
                    PassengerDetailsPanel.Children.Add(new TextBlock { Text = $"Паспорт: {passenger.PassportNumber}" });
                    PassengerDetailsPanel.Children.Add(new TextBlock { Text = $"Дата рождения: {passenger.DateOfBirth:d}" });
                }

                // Класс обслуживания
                var serviceClass = DB.air.ServiceClasses.Find(_booking.ClassId);
                if (serviceClass != null)
                {
                    ServicesPanel.Children.Add(new TextBlock { Text = $"Класс: {serviceClass.ClassName}" });
                }

                // Дополнительные услуги
                var bookingServices = DB.air.BookingServices
                    .Where(bs => bs.BookingId == _booking.BookingId)
                    .ToList();

                var allServices = DB.air.AdditionalServices.ToList();

                foreach (var bs in bookingServices)
                {
                    var service = allServices.FirstOrDefault(s => s.ServiceId == bs.ServiceId);
                    if (service != null)
                    {
                        ServicesPanel.Children.Add(new TextBlock { Text = $"{service.ServiceName} - {service.ServicePrice:C}" });
                    }
                }

                TotalPriceTextBlock.Text = $"{_booking.TotalPrice:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении данных: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем существование бронирования в БД
                var savedBooking = DB.air.Bookings.Find(_booking.BookingId);
                if (savedBooking == null)
                {
                    MessageBox.Show("Ошибка: бронирование не найдено в базе данных");
                    return;
                }

                MessageBox.Show("Бронирование успешно подтверждено!");

                // Переходим на страницу просмотра билета
                NavigationService?.Navigate(new TicketView(_booking.BookingId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подтверждении бронирования: {ex.Message}");
            }
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CustomerMenu(_currentUserId));
        }
    }
}