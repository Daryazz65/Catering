using Catering.AppData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for ManagerReadOnlyUserPage.xaml
    /// </summary>
    public partial class ManagerReadOnlyUserPage : Page
    {
        public ManagerReadOnlyUserPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var context = App.GetContext();
                var users = context.User.Include(u => u.Role)
                    .Where(u => u.Role != null && u.Role.Name == "Клиент")
                    .ToList();

                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить пользователей: " + ex.Message);
            }
        }
    }
}
