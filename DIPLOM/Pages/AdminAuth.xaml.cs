//using System.Windows;
//using System.Windows.Controls;
//using DIPLOM.Connection;
//using System.Linq;

//namespace DIPLOM.Pages
//{
//    public partial class AdminAuth : Page
//    {
//        public AdminAuth()
//        {
//            InitializeComponent();
//        }

//        private void LoginButton_Click(object sender, RoutedEventArgs e)
//        {
//            string login = LoginTextBox.Text;
//            string password = PasswordBox.Password;

//            // Validate inputs
//            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
//            {
//                MessageBox.Show("Пожалуйста, введите логин и пароль");
//                return;
//            }

//            // Check credentials against database
//            var adminUser = DB.air.Users
//                .FirstOrDefault(u => u.Email == login &&
//                                    u.Password == password &&
//                                    u.RoleId == 2 && // Administrator role
//                                    u.IsActive == true);

//            if (adminUser != null)
//            {
//                // Successful login
//                NavigationService.Navigate(new AdminDashboard());
//            }
//            else
//            {
//                MessageBox.Show("Неверный логин или пароль, или у вас нет прав администратора");
//            }
//        }
//    }
//}
using DIPLOM.Connection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class AdminAuth : Page
    {
        public AdminAuth()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            try
            {
                var operatorUser = DB.air.Operators
                    .FirstOrDefault(o => o.Email == login &&
                                       o.Password == password &&
                                       o.IsActive);

                if (operatorUser != null)
                {
                    NavigationService.Navigate(new AdminDashboard());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль, или учетная запись неактивна");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PoskeMainPage());
        }
    }
}