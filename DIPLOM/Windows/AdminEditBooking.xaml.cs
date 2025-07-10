using System;
using System.Linq;
using System.Windows;
using DIPLOM.Connection;

namespace DIPLOM.Windows
{
    public partial class AdminEditBooking : Window
    {
        private int _bookingId;
        private Bookings _currentBooking;

        public AdminEditBooking(int bookingId)
        {
            InitializeComponent();
            _bookingId = bookingId;
            LoadBookingData();
            LoadComboBoxData();
        }

        private void LoadBookingData()
        {
            try
            {
                _currentBooking = DB.air.Bookings.Find(_bookingId);
                if (_currentBooking == null)
                {
                    MessageBox.Show("Бронирование не найдено!");
                    this.Close();
                    return;
                }

                // Заполняем основные данные
                BookingIdTextBlock.Text = _currentBooking.BookingId.ToString();
                CommentsTextBox.Text = _currentBooking.Comments ?? "";
                TotalPriceTextBlock.Text = $"{_currentBooking.TotalPrice} руб.";

                // Загружаем статусы
                StatusComboBox.Items.Add("Confirmed");
                StatusComboBox.Items.Add("Cancelled");
                StatusComboBox.Items.Add("Completed");
                StatusComboBox.SelectedItem = _currentBooking.Status;

                // Загружаем дополнительные услуги
                var services = DB.air.BookingServices
                    .Where(bs => bs.BookingId == _bookingId)
                    .Select(bs => bs.ServiceId)
                    .ToList();

                BaggageCheckBox.IsChecked = services.Contains(1);
                ExtraBaggageCheckBox.IsChecked = services.Contains(2);
                PetInCabinCheckBox.IsChecked = services.Contains(3);
                PetInHoldCheckBox.IsChecked = services.Contains(4);
                InfantCheckBox.IsChecked = services.Contains(5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных бронирования: " + ex.Message);
                this.Close();
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка пользователей
                UserComboBox.ItemsSource = DB.air.Users.ToList();
                UserComboBox.DisplayMemberPath = "FirstName";
                UserComboBox.SelectedValue = _currentBooking.UserId;
                UserComboBox.SelectedValuePath = "UserId";

                // Загрузка рейсов
                FlightComboBox.ItemsSource = DB.air.Flights.ToList();
                FlightComboBox.DisplayMemberPath = "FlightNumber";
                FlightComboBox.SelectedValue = _currentBooking.FlightId;
                FlightComboBox.SelectedValuePath = "FlightId";

                // Загрузка классов обслуживания
                ClassComboBox.ItemsSource = DB.air.ServiceClasses.ToList();
                ClassComboBox.DisplayMemberPath = "ClassName";
                ClassComboBox.SelectedValue = _currentBooking.ClassId;
                ClassComboBox.SelectedValuePath = "ClassId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null ||
                FlightComboBox.SelectedItem == null ||
                ClassComboBox.SelectedItem == null ||
                StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                return;
            }

            try
            {
                // Обновляем основную информацию
                _currentBooking.UserId = ((Users)UserComboBox.SelectedItem).UserId;
                _currentBooking.FlightId = ((Flights)FlightComboBox.SelectedItem).FlightId;
                _currentBooking.ClassId = ((ServiceClasses)ClassComboBox.SelectedItem).ClassId;
                _currentBooking.Status = StatusComboBox.SelectedItem.ToString();
                _currentBooking.Comments = CommentsTextBox.Text;

                // Пересчитываем цену
                var flight = (Flights)FlightComboBox.SelectedItem;
                var serviceClass = (ServiceClasses)ClassComboBox.SelectedItem;
                _currentBooking.TotalPrice = flight.BasePrice * serviceClass.PriceMultiplier;

                // Добавляем стоимость услуг
                if (BaggageCheckBox.IsChecked == true) _currentBooking.TotalPrice += 30;
                if (ExtraBaggageCheckBox.IsChecked == true) _currentBooking.TotalPrice += 50;
                if (PetInCabinCheckBox.IsChecked == true) _currentBooking.TotalPrice += 75;
                if (PetInHoldCheckBox.IsChecked == true) _currentBooking.TotalPrice += 100;
                if (InfantCheckBox.IsChecked == true) _currentBooking.TotalPrice += 25;

                // Обновляем услуги
                UpdateServices();

                DB.air.SaveChanges();

                MessageBox.Show("Изменения сохранены успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateServices()
        {
            // Удаляем все текущие услуги
            var currentServices = DB.air.BookingServices.Where(bs => bs.BookingId == _bookingId).ToList();
            foreach (var service in currentServices)
            {
                DB.air.BookingServices.Remove(service);
            }

            // Добавляем выбранные услуги
            if (BaggageCheckBox.IsChecked == true)
                DB.air.BookingServices.Add(new BookingServices { BookingId = _bookingId, ServiceId = 1, Quantity = 1 });
            if (ExtraBaggageCheckBox.IsChecked == true)
                DB.air.BookingServices.Add(new BookingServices { BookingId = _bookingId, ServiceId = 2, Quantity = 1 });
            if (PetInCabinCheckBox.IsChecked == true)
                DB.air.BookingServices.Add(new BookingServices { BookingId = _bookingId, ServiceId = 3, Quantity = 1 });
            if (PetInHoldCheckBox.IsChecked == true)
                DB.air.BookingServices.Add(new BookingServices { BookingId = _bookingId, ServiceId = 4, Quantity = 1 });
            if (InfantCheckBox.IsChecked == true)
                DB.air.BookingServices.Add(new BookingServices { BookingId = _bookingId, ServiceId = 5, Quantity = 1 });
        }
    }
}