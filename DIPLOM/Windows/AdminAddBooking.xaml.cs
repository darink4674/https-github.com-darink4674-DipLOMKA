using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DIPLOM.Connection;

namespace DIPLOM.Windows
{
    public partial class AdminAddBooking : Window
    {
        public AdminAddBooking()
        {
            InitializeComponent();
            LoadData();
            BookingIdTextBox.Text = (DB.air.Bookings.Count() + 1).ToString();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка пользователей
                UserComboBox.ItemsSource = DB.air.Users
                    .Where(u => u.RoleId == 1) // Только клиенты
                    .ToList();
                UserComboBox.DisplayMemberPath = "FirstName";

                // Загрузка рейсов
                FlightComboBox.ItemsSource = DB.air.Flights
                    .Where(f => f.IsActive)
                    .ToList();
                FlightComboBox.DisplayMemberPath = "FlightNumber";

                // Загрузка классов обслуживания
                ClassComboBox.ItemsSource = DB.air.ServiceClasses.ToList();
                ClassComboBox.DisplayMemberPath = "ClassName";

                // Загрузка дополнительных услуг
                var services = DB.air.AdditionalServices.ToList();
                BaggageCheckBox.Content = $"{services[0].ServiceName} ({services[0].ServicePrice} руб.)";
                ExtraBaggageCheckBox.Content = $"{services[1].ServiceName} ({services[1].ServicePrice} руб.)";
                PetInCabinCheckBox.Content = $"{services[2].ServiceName} ({services[2].ServicePrice} руб.)";
                PetInHoldCheckBox.Content = $"{services[3].ServiceName} ({services[3].ServicePrice} руб.)";
                InfantCheckBox.Content = $"{services[4].ServiceName} ({services[4].ServicePrice} руб.)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка ввода
            if (UserComboBox.SelectedItem == null ||
                FlightComboBox.SelectedItem == null ||
                ClassComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                return;
            }

            try
            {
                var selectedUser = (Users)UserComboBox.SelectedItem;
                var selectedFlight = (Flights)FlightComboBox.SelectedItem;
                var selectedClass = (ServiceClasses)ClassComboBox.SelectedItem;

                // Расчет цены
                decimal totalPrice = selectedFlight.BasePrice * selectedClass.PriceMultiplier;

                // Добавление стоимости услуг
                if (BaggageCheckBox.IsChecked == true)
                    totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 1).ServicePrice;
                if (ExtraBaggageCheckBox.IsChecked == true)
                    totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 2).ServicePrice;
                if (PetInCabinCheckBox.IsChecked == true)
                    totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 3).ServicePrice;
                if (PetInHoldCheckBox.IsChecked == true)
                    totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 4).ServicePrice;
                if (InfantCheckBox.IsChecked == true)
                    totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 5).ServicePrice;

                // Создание бронирования
                var booking = new Bookings
                {
                    UserId = selectedUser.UserId,
                    FlightId = selectedFlight.FlightId,
                    ClassId = selectedClass.ClassId,
                    BookingDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    Status = "Confirmed",
                    IsOperatorBooking = true,
                    Comments = CommentsTextBox.Text
                };

                DB.air.Bookings.Add(booking);
                DB.air.SaveChanges();

                // Добавление услуг
                if (BaggageCheckBox.IsChecked == true)
                    AddService(booking.BookingId, 1);
                if (ExtraBaggageCheckBox.IsChecked == true)
                    AddService(booking.BookingId, 2);
                if (PetInCabinCheckBox.IsChecked == true)
                    AddService(booking.BookingId, 3);
                if (PetInHoldCheckBox.IsChecked == true)
                    AddService(booking.BookingId, 4);
                if (InfantCheckBox.IsChecked == true)
                    AddService(booking.BookingId, 5);

                MessageBox.Show($"Бронирование #{booking.BookingId} успешно создано!\nСтоимость: {totalPrice} руб.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании бронирования: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddService(int bookingId, int serviceId)
        {
            DB.air.BookingServices.Add(new BookingServices
            {
                BookingId = bookingId,
                ServiceId = serviceId,
                Quantity = 1
            });
            DB.air.SaveChanges();
        }

        private void FlightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            try
            {
                if (FlightComboBox.SelectedItem != null && ClassComboBox.SelectedItem != null)
                {
                    var flight = (Flights)FlightComboBox.SelectedItem;
                    var serviceClass = (ServiceClasses)ClassComboBox.SelectedItem;

                    decimal totalPrice = flight.BasePrice * serviceClass.PriceMultiplier;

                    if (BaggageCheckBox.IsChecked == true)
                        totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 1).ServicePrice;
                    if (ExtraBaggageCheckBox.IsChecked == true)
                        totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 2).ServicePrice;
                    if (PetInCabinCheckBox.IsChecked == true)
                        totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 3).ServicePrice;
                    if (PetInHoldCheckBox.IsChecked == true)
                        totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 4).ServicePrice;
                    if (InfantCheckBox.IsChecked == true)
                        totalPrice += DB.air.AdditionalServices.First(s => s.ServiceId == 5).ServicePrice;

                    TotalPriceTextBlock.Text = $"{totalPrice} руб.";
                }
            }
            catch { }
        }
    }
}