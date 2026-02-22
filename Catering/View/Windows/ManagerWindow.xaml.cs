using Catering.View.Pages;
using System.Windows;

namespace Catering.View.Windows
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }
        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagerCrudOrderItemPage());
        }

        private void BtnCategoryDish_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagerReadOnlyCategoryDishPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagerReadOnlyUserPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthorisationWindow();
            auth.Show();
            this.Close();
        }
    }
}
