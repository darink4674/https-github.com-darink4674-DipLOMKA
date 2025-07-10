using DIPLOM.Connection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DIPLOM.Pages
{
    public partial class CustomerAuth : Page
    {
        public CustomerAuth()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите email и пароль");
                return;
            }

            try
            {
                // Ищем пользователя в базе данных
                var user = DB.air.Users.FirstOrDefault(u =>
                    u.Email == email &&
                    u.Password == password &&
                    u.IsActive);

                if (user != null)
                {
                    // Передаем ID пользователя в меню
                    NavigationService.Navigate(new CustomerMenu(user.UserId));
                }
                else
                {
                    MessageBox.Show("Неверный email или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomerRegistration());
        }
    }
}