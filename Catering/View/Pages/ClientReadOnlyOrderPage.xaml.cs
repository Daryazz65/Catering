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
    /// Interaction logic for ClientReadOnlyOrderPage.xaml
    /// </summary>
    public partial class ClientReadOnlyOrderPage : Page
    {
        public ClientReadOnlyOrderPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                var context = App.GetContext();
                var user = App.CurrentUser ?? context.User.FirstOrDefault();
                if (user == null)
                {
                    MessageBoxHelper.Error("Пользователь не найден.");
                    // Очистим UI
                    OrdersDataGrid.ItemsSource = null;
                    ItemsControlOrderItems.ItemsSource = null;
                    StatusText.Text = "";
                    AddressText.Text = "";
                    CommentText.Text = "";
                    return;
                }

                var orders = context.Order
                    .Include(o => o.Status)
                    .Include("OrderItem.Dish")
                    .Where(o => o.IdUser == user.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
                OrdersDataGrid.ItemsSource = orders;

                // Установим выбранный заказ асинхронно, чтобы DataGrid успел привязаться
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (orders.Any())
                    {
                        OrdersDataGrid.SelectedIndex = 0;
                    }
                    else
                    {
                        ItemsControlOrderItems.ItemsSource = null;
                        StatusText.Text = "У вас пока нет заказов.";
                        AddressText.Text = string.Empty;
                        CommentText.Text = string.Empty;
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить заказы: " + ex.Message);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = OrdersDataGrid.SelectedItem as Model.Order;
                if (selected == null)
                {
                    // Очистка деталей
                    StatusText.Text = string.Empty;
                    AddressText.Text = string.Empty;
                    CommentText.Text = string.Empty;
                    ItemsControlOrderItems.ItemsSource = null;
                    return;
                }

                StatusText.Text = selected.Status != null ? selected.Status.Name : selected.OrderStatus.ToString();
                AddressText.Text = selected.DeliveryAddress;
                CommentText.Text = selected.Comment;
                ItemsControlOrderItems.ItemsSource = selected.OrderItem != null ? selected.OrderItem.ToList() : null;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось показать детали заказа: " + ex.Message);
            }
        }
    }
}
