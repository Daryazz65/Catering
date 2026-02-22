using System.Windows;

namespace Catering.View.Windows
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }
        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Pages.ClientAddOrderPage());
        }

        private void BtnMyOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Pages.ClientReadOnlyOrderPage());
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Pages.ProfilePage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthorisationWindow();
            auth.Show();
            this.Close();
        }
    }
}
