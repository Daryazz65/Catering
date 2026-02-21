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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Catering.AppData;
using Catering.Model;
using System.Data.Entity;

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            try
            {
                var context = App.GetContext();
                var user = App.CurrentUser ?? context.User.Include(u => u.Role).FirstOrDefault();
                if (user == null)
                {
                    MessageBoxHelper.Error("Пользователь не найден.");
                    return;
                }

                FullNameText.Text = user.FullName;
                LoginText.Text = user.Login;
                PasswordBox.Password = user.Password;
                PhoneText.Text = user.Phone;
                RoleText.Text = user.Role != null ? user.Role.Name : user.IdRole.ToString();

                var ordersCount = context.Order.Count(o => o.IdUser == user.Id);
                OrdersCountText.Text = ordersCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить профиль: " + ex.Message);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                var user = App.CurrentUser ?? context.User.FirstOrDefault();
                if (user == null)
                {
                    MessageBoxHelper.Error("Пользователь не найден.");
                    return;
                }

                // Валидация уникальности логина
                var newLogin = LoginText.Text.Trim();
                if (!string.IsNullOrWhiteSpace(newLogin))
                {
                    bool conflict = context.User.Any(u => u.Login == newLogin && u.Id != user.Id);
                    if (conflict)
                    {
                        MessageBoxHelper.Error("Пользователь с таким логином уже существует.");
                        return;
                    }
                }

                var tracked = context.User.Find(user.Id);
                if (tracked != null)
                {
                    tracked.FullName = FullNameText.Text.Trim();
                    tracked.Login = newLogin;
                    tracked.Password = PasswordBox.Password;
                    tracked.Phone = PhoneText.Text.Trim();
                }
                else
                {
                    MessageBoxHelper.Error("Пользователь не найден в контексте.");
                    return;
                }

                context.SaveChanges();
                // Обновим App.CurrentUser
                App.CurrentUser = tracked;
                MessageBoxHelper.Information("Профиль сохранён.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить профиль: " + ex.Message);
            }
        }
    }
}
