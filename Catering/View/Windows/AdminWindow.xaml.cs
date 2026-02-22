using System.Windows;

namespace Catering.View.Windows
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }
        private void BtnCategoryDish_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Pages.AdminCrudCategoryDishPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Pages.AdminCrudUserPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthorisationWindow();
            auth.Show();
            this.Close();
        }
    }
}
