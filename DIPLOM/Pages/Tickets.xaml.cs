
//using System;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using DIPLOM.Connection;

//namespace DIPLOM.Pages
//{
//    public partial class Tickets : Page
//    {
//        private int _currentUserId; // Добавляем поле для хранения ID текущего пользователя

//        public Tickets() // Добавляем параметр в конструктор
//        {
//            InitializeComponent();

//            LoadUserTickets(); // Переименовываем метод для ясности
//        }

//        private void LoadUserTickets()
//        {
//            try
//            {
//                // Получаем бронирования только для текущего пользователя
//                var bookings = DB.air.Bookings
//                    .Where(b => b.UserId == _currentUserId) // Фильтрация по ID пользователя
//                    .OrderByDescending(b => b.BookingDate)
//                    .ToList();

//                TicketsListView.ItemsSource = bookings;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при загрузке билетов: {ex.Message}");
//            }
//        }

//        private void BackButton_Click(object sender, RoutedEventArgs e)
//        {
//            NavigationService.GoBack();
//        }

//        private void ViewDetails_Click(object sender, RoutedEventArgs e)
//        {
//            var button = sender as Button;
//            if (button != null && button.Tag is int bookingId)
//            {
//                // Получаем полную информацию о бронировании
//                var booking = DB.air.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
//                if (booking != null)
//                {
//                    // Получаем информацию о рейсе
//                    var flight = DB.air.Flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
//                    // Получаем информацию о классе обслуживания
//                    var serviceClass = DB.air.ServiceClasses.FirstOrDefault(sc => sc.ClassId == booking.ClassId);
//                    // Получаем пассажиров
//                    var passengers = DB.air.Passengers.Where(p => p.BookingId == bookingId).ToList();
//                    // Получаем дополнительные услуги
//                    var services = DB.air.BookingServices
//                        .Where(bs => bs.BookingId == bookingId)
//                        .Join(DB.air.AdditionalServices,
//                            bs => bs.ServiceId,
//                            s => s.ServiceId,
//                            (bs, s) => new { Service = s, Quantity = bs.Quantity })
//                        .ToList();

//                    string message = $"Детали бронирования #{bookingId}\n\n";
//                    message += $"Номер рейса: {flight?.FlightNumber}\n";
//                    message += $"Дата вылета: {flight?.DepartureTime:dd.MM.yyyy HH:mm}\n";
//                    message += $"Класс: {serviceClass?.ClassName}\n";
//                    message += $"Стоимость: {booking.TotalPrice} руб.\n";
//                    message += $"Статус: {booking.Status}\n\n";
//                    message += $"Пассажиры ({passengers.Count}):\n";

//                    foreach (var passenger in passengers)
//                    {
//                        message += $"- {passenger.LastName} {passenger.FirstName} (Паспорт: {passenger.PassportNumber})\n";
//                    }

//                    if (services.Any())
//                    {
//                        message += "\nДополнительные услуги:\n";
//                        foreach (var service in services)
//                        {
//                            message += $"- {service.Service.ServiceName} (x{service.Quantity}) - {service.Service.ServicePrice * service.Quantity} руб.\n";
//                        }
//                    }

//                    MessageBox.Show(message, "Детали бронирования");
//                }
//            }
//        }

//        private void DeleteTicket_Click(object sender, RoutedEventArgs e)
//        {
//            var button = sender as Button;
//            if (button != null && button.Tag is int bookingId)
//            {
//                var result = MessageBox.Show("Вы действительно хотите отменить этот билет?",
//                    "Подтверждение отмены", MessageBoxButton.YesNo);

//                if (result == MessageBoxResult.Yes)
//                {
//                    try
//                    {
//                        var booking = DB.air.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
//                        if (booking != null && booking.UserId == _currentUserId) // Проверяем, что билет принадлежит пользователю
//                        {
//                            booking.Status = "Cancelled";
//                            DB.air.SaveChanges();
//                            LoadUserTickets(); // Обновляем список
//                            MessageBox.Show("Билет успешно отменен");
//                        }
//                        else
//                        {
//                            MessageBox.Show("Вы не можете отменить этот билет.");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Ошибка при отмене билета: {ex.Message}");
//                    }
//                }
//            }
//        }
//    }
//}
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DIPLOM.Connection;

namespace DIPLOM.Pages
{
    public partial class Tickets : Page
    {
        private readonly int _currentUserId;

        public Tickets(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            LoadUserTickets();
        }

        private void LoadUserTickets()
        {
            try
            {
                var bookings = DB.air.Bookings
                    .Where(b => b.UserId == _currentUserId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                TicketsListView.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке билетов: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int bookingId)
            {
                var booking = DB.air.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (booking != null)
                {
                    var flight = DB.air.Flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
                    var serviceClass = DB.air.ServiceClasses.FirstOrDefault(sc => sc.ClassId == booking.ClassId);
                    var passengers = DB.air.Passengers.Where(p => p.BookingId == bookingId).ToList();
                    var services = DB.air.BookingServices
                        .Where(bs => bs.BookingId == bookingId)
                        .Join(DB.air.AdditionalServices,
                            bs => bs.ServiceId,
                            s => s.ServiceId,
                            (bs, s) => new { Service = s, Quantity = bs.Quantity })
                        .ToList();

                    string message = $"Детали бронирования #{bookingId}\n\n";
                    message += $"Номер рейса: {flight?.FlightNumber}\n";
                    message += $"Дата вылета: {flight?.DepartureTime:dd.MM.yyyy HH:mm}\n";
                    message += $"Класс: {serviceClass?.ClassName}\n";
                    message += $"Стоимость: {booking.TotalPrice} руб.\n";
                    message += $"Статус: {booking.Status}\n\n";
                    message += $"Пассажиры ({passengers.Count}):\n";

                    foreach (var passenger in passengers)
                    {
                        message += $"- {passenger.LastName} {passenger.FirstName} (Паспорт: {passenger.PassportNumber})\n";
                    }

                    if (services.Any())
                    {
                        message += "\nДополнительные услуги:\n";
                        foreach (var service in services)
                        {
                            message += $"- {service.Service.ServiceName} (x{service.Quantity}) - {service.Service.ServicePrice * service.Quantity} руб.\n";
                        }
                    }

                    MessageBox.Show(message, "Детали бронирования");
                }
            }
        }

        private void DeleteTicket_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int bookingId)
            {
                var result = MessageBox.Show("Вы действительно хотите отменить этот билет?",
                    "Подтверждение отмены", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var booking = DB.air.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
                        if (booking != null && booking.UserId == _currentUserId)
                        {
                            booking.Status = "Cancelled";
                            DB.air.SaveChanges();
                            LoadUserTickets();
                            MessageBox.Show("Билет успешно отменен");
                        }
                        else
                        {
                            MessageBox.Show("Вы не можете отменить этот билет.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при отмене билета: {ex.Message}");
                    }
                }
            }
        }
    }
}