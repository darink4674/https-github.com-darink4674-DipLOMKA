using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DIPLOM.Connection;

namespace DIPLOM.Pages
{
    public partial class TicketView : Page
    {
        private int _bookingId;

        public TicketView(int bookingId)
        {
            InitializeComponent();
            _bookingId = bookingId;
            LoadTicketData();
        }

        private void LoadTicketData()
        {
            try
            {
                var bookingInfo = (from b in DB.air.Bookings
                                   join f in DB.air.Flights on b.FlightId equals f.FlightId
                                   join sc in DB.air.ServiceClasses on b.ClassId equals sc.ClassId
                                   join da in DB.air.Airports on f.DepartureAirportId equals da.AirportId
                                   join aa in DB.air.Airports on f.ArrivalAirportId equals aa.AirportId
                                   where b.BookingId == _bookingId
                                   select new
                                   {
                                       b.BookingId,
                                       f.FlightNumber,
                                       DepartureCity = da.City,
                                       ArrivalCity = aa.City,
                                       f.DepartureTime,
                                       sc.ClassName,
                                       b.TotalPrice,
                                       Insurance = DB.air.Insurance.FirstOrDefault(i => i.BookingId == b.BookingId),
                                       Baggage = DB.air.BaggageReceipts.FirstOrDefault(br => br.BookingId == b.BookingId)
                                   }).FirstOrDefault();

                if (bookingInfo != null)
                {
                    FlightNumberTextBlock.Text = bookingInfo.FlightNumber;
                    RouteTextBlock.Text = $"{bookingInfo.DepartureCity} → {bookingInfo.ArrivalCity}";
                    DepartureTextBlock.Text = bookingInfo.DepartureTime.ToString("g");
                    ClassTextBlock.Text = bookingInfo.ClassName;
                    PriceTextBlock.Text = $"{bookingInfo.TotalPrice} руб.";

                    if (bookingInfo.Insurance != null)
                    {
                        InsuranceTextBlock.Text = $"Страховка: {bookingInfo.Insurance.InsuranceType} ({bookingInfo.Insurance.Price} руб.)";
                    }

                    if (bookingInfo.Baggage != null)
                    {
                        BaggageTextBlock.Text = $"Багаж: {bookingInfo.Baggage.BaggageCount} мест, {bookingInfo.Baggage.TotalWeight} кг";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных билета: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PoskeMainPage());
        }
    }
}