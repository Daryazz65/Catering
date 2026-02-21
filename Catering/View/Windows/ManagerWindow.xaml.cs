using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Catering.View.Pages;
using System.Linq;
using Catering.View.Windows;
using Catering.View.Pages;
using System;

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
            // Переход на страницу управления заказами и позициями
            MainFrame.Navigate(new ManagerCrudOrderItemPage());
        }

        private void BtnCategoryDish_Click(object sender, RoutedEventArgs e)
        {
            // Просмотр категорий и блюд
            MainFrame.Navigate(new ManagerReadOnlyCategoryDishPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            // Просмотр пользователей (без админов и менеджеров)
            MainFrame.Navigate(new ManagerReadOnlyUserPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Вернуться на окно авторизации
            var auth = new AuthorisationWindow();
            auth.Show();
            this.Close();
        }
    }
}
