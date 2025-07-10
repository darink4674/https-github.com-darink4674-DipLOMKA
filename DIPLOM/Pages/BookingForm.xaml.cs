using DIPLOM.Connection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class BookingForm : Page
    {
        private readonly Flights _selectedFlight;
        private readonly int _currentUserId;

        public BookingForm(Flights selectedFlight, int userId)
        {
            InitializeComponent();
            _selectedFlight = selectedFlight;
            _currentUserId = userId;
            LoadData();
            DisplayFlightInfo();
        }

        private void LoadData()
        {
            try
            {
                ClassComboBox.ItemsSource = DB.air.ServiceClasses.ToList();
                ClassComboBox.SelectedIndex = 0;
                BasePriceTextBlock.Text = $"{_selectedFlight.BasePrice} руб.";
                CalculateTotalPrice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void DisplayFlightInfo()
        {
            try
            {
                var departure = DB.air.Airports.Find(_selectedFlight.DepartureAirportId);
                var arrival = DB.air.Airports.Find(_selectedFlight.ArrivalAirportId);
                var aircraft = DB.air.Aircrafts.Find(_selectedFlight.AircraftId);

                if (departure == null || arrival == null || aircraft == null)
                {
                    MessageBox.Show("Ошибка загрузки информации о рейсе");
                    return;
                }

                FlightInfoTextBlock.Text =
                    $"Рейс: {_selectedFlight.FlightNumber}\n" +
                    $"Откуда: {departure.City} ({departure.AirportCode})\n" +
                    $"Куда: {arrival.City} ({arrival.AirportCode})\n" +
                    $"Вылет: {_selectedFlight.DepartureTime:g}\n" +
                    $"Прибытие: {_selectedFlight.ArrivalTime:g}\n" +
                    $"Самолет: {aircraft.Model}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения информации: {ex.Message}");
            }
        }

        private decimal CalculateTotalPrice()
        {
            decimal total = _selectedFlight.BasePrice;

            if (ClassComboBox.SelectedItem is ServiceClasses selectedClass)
            {
                total *= selectedClass.PriceMultiplier;
            }

            // Обновленные цены в рублях
            if (ExtraBaggageCheckBox.IsChecked == true) total += 4000;
            if (PetInCabinCheckBox.IsChecked == true) total += 6000;
            if (PetInHoldCheckBox.IsChecked == true) total += 8000;

            TotalPriceTextBlock.Text = $"{total} руб.";
            return total;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                // Создаем бронирование
                var booking = new Bookings
                {
                    FlightId = _selectedFlight.FlightId,
                    ClassId = ((ServiceClasses)ClassComboBox.SelectedItem).ClassId,
                    BookingDate = DateTime.Now,
                    TotalPrice = CalculateTotalPrice(),
                    Status = "Confirmed",
                    UserId = _currentUserId,
                    IsOperatorBooking = false
                };

                DB.air.Bookings.Add(booking);
                DB.air.SaveChanges();

                // Создаем пассажира
                var passenger = new Passengers
                {
                    BookingId = booking.BookingId,
                    FirstName = FirstNameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    PassportNumber = PassportNumberTextBox.Text.Trim(),
                    DateOfBirth = BirthDatePicker.SelectedDate.Value
                };

                DB.air.Passengers.Add(passenger);

                // Добавляем страховку (5% от стоимости билета)
                var insurance = new Insurance
                {
                    BookingId = booking.BookingId,
                    InsuranceType = "Стандартная",
                    Price = booking.TotalPrice * 0.05m,
                    Details = "Страховка на время полета"
                };
                DB.air.Insurance.Add(insurance);

                // Добавляем багажную квитанцию
                int baggageCount = 0;
                decimal baggageWeight = 0;

                //if (BaggageCheckBox.IsChecked == true)
                //{
                //    baggageCount++;
                //    baggageWeight += 20;
                //    DB.air.BookingServices.Add(new BookingServices { BookingId = booking.BookingId, ServiceId = 1 });
                //}
                if (ExtraBaggageCheckBox.IsChecked == true)
                {
                    baggageCount++;
                    baggageWeight += 10; // дополнительный вес
                    DB.air.BookingServices.Add(new BookingServices { BookingId = booking.BookingId, ServiceId = 2 });
                }
                if (PetInCabinCheckBox.IsChecked == true)
                {
                    DB.air.BookingServices.Add(new BookingServices { BookingId = booking.BookingId, ServiceId = 3 });
                }
                if (PetInHoldCheckBox.IsChecked == true)
                {
                    DB.air.BookingServices.Add(new BookingServices { BookingId = booking.BookingId, ServiceId = 4 });
                }
              

                if (baggageCount > 0)
                {
                    var baggageReceipt = new BaggageReceipts
                    {
                        BookingId = booking.BookingId,
                        BaggageCount = baggageCount,
                        TotalWeight = baggageWeight,
                        Details = $"Багаж для {passenger.FirstName} {passenger.LastName}"
                    };
                    DB.air.BaggageReceipts.Add(baggageReceipt);
                }

                DB.air.SaveChanges();

                NavigationService.Navigate(new BookingConfirmation(booking, _currentUserId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при бронировании: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя пассажира");
                FirstNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию пассажира");
                LastNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PassportNumberTextBox.Text))
            {
                MessageBox.Show("Введите номер паспорта");
                PassportNumberTextBox.Focus();
                return false;
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения");
                BirthDatePicker.Focus();
                return false;
            }

            if (BirthDatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Дата рождения не может быть в будущем");
                BirthDatePicker.Focus();
                return false;
            }

            return true;
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotalPrice();
        }
    }
}