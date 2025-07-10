using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DIPLOM.Connection;
using DIPLOM.Windows;

namespace DIPLOM.Pages
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();
            LoadAllBookings();
        }

        private void LoadAllBookings()
        {
            try
            {
                var bookings = (
                    from b in DB.air.Bookings
                    join u in DB.air.Users on b.UserId equals u.UserId
                    join f in DB.air.Flights on b.FlightId equals f.FlightId
                    join sc in DB.air.ServiceClasses on b.ClassId equals sc.ClassId
                    join da in DB.air.Airports on f.DepartureAirportId equals da.AirportId
                    join aa in DB.air.Airports on f.ArrivalAirportId equals aa.AirportId
                    select new
                    {
                        b.BookingId,
                        Username = u.FirstName + " " + u.LastName,
                        f.FlightNumber,
                        DepartureCity = da.City,
                        ArrivalCity = aa.City,
                        f.DepartureTime,
                        sc.ClassName,
                        b.TotalPrice,
                        b.Status,
                        PassengersCount = DB.air.Passengers.Count(p => p.BookingId == b.BookingId),
                        b.BookingDate
                    }).OrderByDescending(b => b.BookingDate).ToList();

                BookingsDataGrid.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки бронирований: " + ex.Message);
            }
        }
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int bookingId)
            {
                var editWindow = new AdminEditBooking(bookingId);
                if (editWindow.ShowDialog() == true)
                {
                    LoadAllBookings(); 
                }
            }
        }

        private void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int bookingId)
            {
                if (MessageBox.Show("Отменить бронирование?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var booking = DB.air.Bookings.Find(bookingId);
                    booking.Status = "Cancelled";
                    DB.air.SaveChanges();
                    LoadAllBookings();
                    MessageBox.Show("Бронирование отменено");
                }
            }
        }

        private void AddBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var addBookingWindow = new AdminAddBooking();
            if (addBookingWindow.ShowDialog() == true)
            {
                LoadAllBookings();
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PoskeMainPage());
        }
    }
}
