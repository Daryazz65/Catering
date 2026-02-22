using Catering.AppData;
using Catering.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for AdminCrudUserPage.xaml
    /// </summary>
    public partial class AdminCrudUserPage : Page
    {
        public AdminCrudUserPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var context = App.GetContext();
            var roles = context.Role.ToList();
            var users = context.User.ToList();

            var comboCol = UsersDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault();
            if (comboCol != null)
            {
                comboCol.ItemsSource = roles;
            }

            UsersDataGrid.ItemsSource = users;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void UsersDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction != DataGridEditAction.Commit)
                {
                    return;
                }

                var edited = e.Row.Item as User;
                if (edited == null)
                    return;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var context = App.GetContext();

                        if (!string.IsNullOrWhiteSpace(edited.Login))
                        {
                            bool conflict = context.User.Any(u => u.Login == edited.Login && u.Id != edited.Id);
                            if (conflict)
                            {
                                MessageBoxHelper.Error("Пользователь с таким логином уже существует.");
                                LoadUsers();
                                return;
                            }
                        }

                        if (edited.Id == 0)
                        {
                            context.User.Add(edited);
                        }
                        else
                        {
                            var tracked = context.User.Find(edited.Id);
                            if (tracked != null)
                            {
                                tracked.FullName = edited.FullName;
                                tracked.Login = edited.Login;
                                tracked.Password = edited.Password;
                                tracked.Phone = edited.Phone;
                                tracked.IdRole = edited.IdRole;
                            }
                            else
                            {
                                context.User.Attach(edited);
                                context.Entry(edited).State = EntityState.Modified;
                            }
                        }

                        context.SaveChanges();
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.Error("Не удалось сохранить изменения: " + ex.Message);
                        LoadUsers();
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения: " + ex.Message);
            }
        }

        private void DeleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as User;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите пользователя для удаления.");
                return;
            }
            var context = App.GetContext();
            try
            {
                context.User.Remove(context.User.First(u => u.Id == selected.Id));
                context.SaveChanges();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось удалить пользователя: " + ex.Message);
            }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var context = App.GetContext();
            try
            {
                var defaultRole = context.Role.FirstOrDefault();
                User user = new User
                {
                    FullName = "Новый пользователь",
                    Login = "new_login",
                    Password = "pass",
                    Phone = "000",
                    IdRole = defaultRole != null ? defaultRole.Id : 1
                };
                context.User.Add(user);
                context.SaveChanges();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось добавить пользователя: " + ex.Message);
            }
        }
    }
}
