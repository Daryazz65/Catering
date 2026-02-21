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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for ManagerCrudOrderItemPage.xaml
    /// </summary>
    public partial class ManagerCrudOrderItemPage : Page
    {
        public ManagerCrudOrderItemPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var context = App.GetContext();
                var dishes = context.Dish.ToList();
                var orders = context.Order.ToList();
                var items = context.OrderItem.ToList();
                var statuses = context.Status.ToList();
                var users = context.User.ToList();

                // OrderItems combobox sources
                var dishCol = OrderItemsDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault(c => c.Header.ToString() == "Блюдо");
                if (dishCol != null)
                    dishCol.ItemsSource = dishes;

                var orderCol = OrderItemsDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault(c => c.Header.ToString() == "Заказ");
                if (orderCol != null)
                    orderCol.ItemsSource = orders;

                OrderItemsDataGrid.ItemsSource = items;

                // Orders grid
                OrdersDataGrid.ItemsSource = orders;
                var statusCol = OrdersDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault(c => c.Header.ToString() == "Статус");
                if (statusCol != null)
                    statusCol.ItemsSource = statuses;

                var userCol = OrdersDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault(c => c.Header.ToString() == "Пользователь");
                if (userCol != null)
                    userCol.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить элементы заказа: " + ex.Message);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void RefreshOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = OrdersDataGrid.SelectedItem as Order;
                var context = App.GetContext();
                if (selected == null)
                {
                    OrderItemsDataGrid.ItemsSource = context.OrderItem.ToList();
                }
                else
                {
                    OrderItemsDataGrid.ItemsSource = context.OrderItem.Where(oi => oi.IdOrder == selected.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить элементы заказа: " + ex.Message);
            }
        }

        private void OrdersDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction != DataGridEditAction.Commit)
                    return;

                var edited = e.Row.Item as Order;
                if (edited == null)
                    return;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var context = App.GetContext();

                        if (edited.OrderDate == default(DateTime))
                            edited.OrderDate = DateTime.Now;

                        if (edited.Id == 0)
                        {
                            context.Order.Add(edited);
                        }
                        else
                        {
                            var tracked = context.Order.Find(edited.Id);
                            if (tracked != null)
                            {
                                tracked.OrderDate = edited.OrderDate;
                                tracked.TotalSum = edited.TotalSum;
                                tracked.DeliveryAddress = edited.DeliveryAddress;
                                tracked.Comment = edited.Comment;
                                tracked.OrderStatus = edited.OrderStatus;
                                tracked.IdUser = edited.IdUser;
                            }
                            else
                            {
                                context.Order.Attach(edited);
                                context.Entry(edited).State = EntityState.Modified;
                            }
                        }

                        context.SaveChanges();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.Error("Не удалось сохранить изменения заказа: " + ex.Message);
                        LoadData();
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения заказа: " + ex.Message);
            }
        }

        private void OrderItemsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction != DataGridEditAction.Commit)
                    return;

                var edited = e.Row.Item as OrderItem;
                if (edited == null)
                    return;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var context = App.GetContext();

                        // Валидация количества
                        if (edited.Quantity <= 0)
                        {
                            MessageBoxHelper.Error("Количество должно быть больше 0.");
                            LoadData();
                            return;
                        }

                        // Подстановка цены из блюда, если не указана
                        if (edited.PriceAtMoment <= 0)
                        {
                            var dish = context.Dish.Find(edited.IdDish);
                            if (dish != null)
                                edited.PriceAtMoment = dish.Price;
                        }

                        if (edited.Id == 0)
                        {
                            context.OrderItem.Add(edited);
                        }
                        else
                        {
                            var tracked = context.OrderItem.Find(edited.Id);
                            if (tracked != null)
                            {
                                tracked.IdOrder = edited.IdOrder;
                                tracked.IdDish = edited.IdDish;
                                tracked.Quantity = edited.Quantity;
                                tracked.PriceAtMoment = edited.PriceAtMoment;
                            }
                            else
                            {
                                context.OrderItem.Attach(edited);
                                context.Entry(edited).State = EntityState.Modified;
                            }
                        }

                        context.SaveChanges();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.Error("Не удалось сохранить изменения: " + ex.Message);
                        LoadData();
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения: " + ex.Message);
            }
        }

        private void DeleteOrderItemBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrderItemsDataGrid.SelectedItem as OrderItem;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите элемент заказа для удаления.");
                return;
            }
            try
            {
                var context = App.GetContext();
                context.OrderItem.Remove(context.OrderItem.First(x => x.Id == selected.Id));
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось удалить элемент заказа: " + ex.Message);
            }
        }

        private void AddOrderItemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                var firstOrder = context.Order.FirstOrDefault();
                var firstDish = context.Dish.FirstOrDefault();
                if (firstOrder == null || firstDish == null)
                {
                    MessageBoxHelper.Error("Не удалось добавить элемент: отсутствуют заказы или блюда.");
                    return;
                }

                var item = new OrderItem
                {
                    IdOrder = firstOrder.Id,
                    IdDish = firstDish.Id,
                    Quantity = 1,
                    PriceAtMoment = firstDish.Price
                };
                context.OrderItem.Add(item);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось добавить элемент заказа: " + ex.Message);
            }
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                var firstUser = context.User.FirstOrDefault();
                var firstStatus = context.Status.FirstOrDefault();
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalSum = 0,
                    DeliveryAddress = "",
                    Comment = "",
                    OrderStatus = firstStatus != null ? firstStatus.Id : 1,
                    IdUser = firstUser != null ? firstUser.Id : 1
                };
                context.Order.Add(order);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось добавить заказ: " + ex.Message);
            }
        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersDataGrid.SelectedItem as Order;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите заказ для удаления.");
                return;
            }
            try
            {
                var context = App.GetContext();
                var toDelete = context.Order.Include(o => o.OrderItem).First(o => o.Id == selected.Id);
                // Удалим связанные элементы
                foreach (var oi in toDelete.OrderItem.ToList())
                {
                    context.OrderItem.Remove(oi);
                }
                context.Order.Remove(toDelete);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось удалить заказ: " + ex.Message);
            }
        }
    }
}
