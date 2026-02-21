using Catering.AppData;
using Catering.Model;
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

namespace Catering.View.Windows
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.Information("Поля для регистрации: ФИО, уникальный логин, пароль (не более 10 символов), телефон (не более 20 символов).");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthorisationWindow auth = new AuthorisationWindow();
            auth.Show();
            this.Close();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTb.Text?.Trim();
            string login = LoginTb.Text?.Trim();
            string password = PasswordTb.Password;
            string phone = PhoneTb.Text?.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBoxHelper.Error("Все поля должны быть заполнены.");
                return;
            }

            if (password.Length > 10)
            {
                MessageBoxHelper.Error("Пароль не должен превышать 10 символов.");
                return;
            }

            if (phone.Length > 20)
            {
                MessageBoxHelper.Error("Телефон не должен превышать 20 символов.");
                return;
            }

            var context = App.GetContext();

            var exists = context.User.FirstOrDefault(u => u.Login == login);
            if (exists != null)
            {
                MessageBoxHelper.Error("Пользователь с таким логином уже существует.");
                return;
            }

            var clientRole = context.Role.FirstOrDefault(r => r.Name.Trim().ToLower().Contains("клиент"));
            if (clientRole == null)
            {
                clientRole = context.Role.FirstOrDefault(); 
            }

            int roleId = clientRole != null ? clientRole.Id : 1;

            try
            {
                User user = new User()
                {
                    FullName = fullName,
                    Login = login,
                    Password = password,
                    Phone = phone,
                    IdRole = roleId
                };

                context.User.Add(user);
                context.SaveChanges();

                MessageBoxHelper.Information("Регистрация прошла успешно. Выполните вход под своим аккаунтом.");
                AuthorisationWindow auth = new AuthorisationWindow();
                auth.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось зарегистрировать пользователя. Проверьте правильность введённых данных и подключение к базе.\n" + ex.Message);
            }
        }
    }
}
