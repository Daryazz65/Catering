using Catering.AppData;
using Catering.Model;
using System.Data.Entity;
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
            // Загрузка списка пользователей (lazy loading для навигационных свойств включена в контексте)
            // Получаем роли и пользователей из одного контекста, чтобы сущности были отслеживаемыми
            var roles = context.Role.ToList();
            var users = context.User.ToList();

            // Привязываем список ролей к колонке ComboBox
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
            // Сохранить изменения, внесённые в строку
            try
            {
                if (e.EditAction != DataGridEditAction.Commit)
                {
                    return;
                }

                // Сохраняем изменения асинхронно после завершения редактирования строки,
                // чтобы избежать рекурсивного вызова RowEditEnding (CommitEdit внутри обработчика).
                var edited = e.Row.Item as User;
                if (edited == null)
                    return;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var context = App.GetContext();

                        // Валидация: логин должен быть уникален
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
                            // Новый пользователь
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
            // Простейшее добавление с дефолтными значениями. При необходимости можно открыть диалог ввода.
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
