using System.Windows;
using System.Windows.Controls;

namespace DIPLOM.Pages
{
    public partial class PoskeMainPage : Page
    {
        public PoskeMainPage()
        {
            InitializeComponent();
        }

        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomerAuth());
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminAuth());
        }
    }
}