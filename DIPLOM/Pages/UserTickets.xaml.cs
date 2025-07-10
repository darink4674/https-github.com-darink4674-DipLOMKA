using DIPLOM.Connection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class UserTickets : Page
    {
        private int _userId;

        public UserTickets(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadTickets();
        }

        private void LoadTickets()
        {
            try
            {
                var tickets = (
                    from b in DB.air.Bookings
                    join f in DB.air.Flights on b.FlightId equals f.FlightId
                    join da in DB.air.Airports on f.DepartureAirportId equals da.AirportId
                    join aa in DB.air.Airports on f.ArrivalAirportId equals aa.AirportId
                    join sc in DB.air.ServiceClasses on b.ClassId equals sc.ClassId
                    where b.UserId == _userId && b.Status == "Confirmed"
                    select new
                    {
                        b.BookingId,
                        f.FlightNumber,
                        Route = da.City + " → " + aa.City,
                        f.DepartureTime,
                        Class = sc.ClassName,
                        b.TotalPrice
                    }).OrderByDescending(t => t.DepartureTime).ToList();

                TicketsListView.ItemsSource = tickets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки билетов: {ex.Message}");
            }
        }

        private void ViewTicket_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int bookingId)
            {
                NavigationService.Navigate(new TicketView(bookingId));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomerMenu(_userId));
        }
    }
}