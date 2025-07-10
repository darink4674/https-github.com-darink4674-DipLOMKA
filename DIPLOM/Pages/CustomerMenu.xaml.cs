//using System.Windows;
//using System.Windows.Controls;

//namespace DIPLOM.Pages
//{
//    public partial class CustomerMenu : Page
//    {
//        private int currentUserId;

//        public CustomerMenu(int userId)
//        {
//            InitializeComponent();
//            currentUserId = userId;
//        }

//        private void BuyTicketButton_Click(object sender, RoutedEventArgs e)
//        {
//            NavigationService.Navigate(new FlightSearch(currentUserId));
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
    public partial class CustomerMenu : Page
    {
        private int _userId;

        public CustomerMenu(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void BuyTicketButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FlightSearch(_userId));
        }

        private void ViewTicketsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tickets = (from b in DB.air.Bookings
                               join f in DB.air.Flights on b.FlightId equals f.FlightId
                               join sc in DB.air.ServiceClasses on b.ClassId equals sc.ClassId
                               join da in DB.air.Airports on f.DepartureAirportId equals da.AirportId
                               join aa in DB.air.Airports on f.ArrivalAirportId equals aa.AirportId
                               where b.UserId == _userId
                               select new
                               {
                                   b.BookingId,
                                   FlightNumber = f.FlightNumber,
                                   DepartureCity = da.City,
                                   ArrivalCity = aa.City,
                                   f.DepartureTime,
                                   ClassName = sc.ClassName,
                                   b.TotalPrice
                               }).ToList();

                TicketsListView.ItemsSource = tickets;
                TicketsListView.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Visible;
                BuyTicketButton.Visibility = Visibility.Collapsed;
                ViewTicketsButton.Visibility = Visibility.Collapsed;
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
                NavigationService?.Navigate(new TicketView(bookingId));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TicketsListView.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            BuyTicketButton.Visibility = Visibility.Visible;
            ViewTicketsButton.Visibility = Visibility.Visible;
        }
    }
}