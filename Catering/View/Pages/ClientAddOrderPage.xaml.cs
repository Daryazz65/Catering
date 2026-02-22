using Catering.AppData;
using Catering.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for ClientAddOrderPage.xaml
    /// </summary>
    public partial class ClientAddOrderPage : Page
    {
        private List<Category> _categories;
        private List<Dish> _dishes;
        private ObservableCollection<CartItem> _cart = new ObservableCollection<CartItem>();

        public ClientAddOrderPage()
        {
            InitializeComponent();
            LoadData();
            CartItemsControl.ItemsSource = _cart;
            DishesListBox.AddHandler(Button.ClickEvent, new RoutedEventHandler(DishesListBox_ButtonClick));
        }

        private void DishesListBox_ButtonClick(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            if (btn == null) return;
            var action = btn.Tag as string;
            var dish = btn.DataContext as Dish;
            if (dish == null) return;

            if (action == "View")
            {
                var info = $"{dish.Name}\n\n{dish.Description}\n\nЦена: {dish.Price:0.##} руб.\n{(dish.IsAvailable ? "Доступно" : "Недоступно")}";
                MessageBoxHelper.Information(info);
            }
            else if (action == "Quick")
            {
                var existing = _cart.FirstOrDefault(c => c.Dish.Id == dish.Id);
                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    _cart.Add(new CartItem { Dish = dish, Quantity = 1, Price = dish.Price });
                }
                RefreshCart();
            }
        }

        private void LoadData()
        {
            try
            {
                var context = App.GetContext();
                _categories = context.Category.ToList();
                _dishes = context.Dish.Where(d => d.IsAvailable).ToList();

                RenderCategories();
                RenderDishes(_dishes);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить данные: " + ex.Message);
            }
        }

        private void RenderCategories()
        {
            CategoriesPanel.Children.Clear();

            var allBtn = new Button { Content = "Все", Margin = new Thickness(4, 0, 4, 0), Padding = new Thickness(8, 4, 8, 4) };
            allBtn.Click += (s, e) => RenderDishes(_dishes);
            CategoriesPanel.Children.Add(allBtn);

            foreach (var c in _categories)
            {
                var btn = new Button { Content = c.Name, Tag = c, Margin = new Thickness(4, 0, 4, 0), Padding = new Thickness(8, 4, 8, 4) };
                btn.Click += (s, e) =>
                {
                    var cat = (s as Button).Tag as Category;
                    var filtered = _dishes.Where(d => d.IdCategory == cat.Id).ToList();
                    RenderDishes(filtered);
                };
                CategoriesPanel.Children.Add(btn);
            }
        }

        private void RenderDishes(List<Dish> dishes)
        {
            DishesListBox.ItemsSource = dishes;
        }

        public void ViewDish_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var dish = btn.DataContext as Dish;
            if (dish == null) return;

            var info = $"{dish.Name}\n\n{dish.Description}\n\nЦена: {dish.Price:0.##} руб.\n{(dish.IsAvailable ? "Доступно" : "Недоступно")}";
            MessageBoxHelper.Information(info);
        }

        public void QuickAdd_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var dish = btn.DataContext as Dish;
            if (dish == null) return;

            var existing = _cart.FirstOrDefault(c => c.Dish.Id == dish.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _cart.Add(new CartItem { Dish = dish, Quantity = 1, Price = dish.Price });
            }
            RefreshCart();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var dish = (sender as Button).Tag as Dish;
            if (dish == null) return;

            var existing = _cart.FirstOrDefault(c => c.Dish.Id == dish.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _cart.Add(new CartItem { Dish = dish, Quantity = 1, Price = dish.Price });
            }
            RefreshCart();
        }

        private void IncreaseQty_Click(object sender, RoutedEventArgs e)
        {
            var ci = (sender as Button).Tag as CartItem;
            if (ci == null) return;
            ci.Quantity++;
            RefreshCart();
        }

        private void DecreaseQty_Click(object sender, RoutedEventArgs e)
        {
            var ci = (sender as Button).Tag as CartItem;
            if (ci == null) return;
            if (ci.Quantity > 1) ci.Quantity--; else _cart.Remove(ci);
            RefreshCart();
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var ci = (sender as Button).Tag as CartItem;
            if (ci == null) return;
            _cart.Remove(ci);
            RefreshCart();
        }

        private void RefreshCart()
        {
            CartItemsControl.ItemsSource = null;
            CartItemsControl.ItemsSource = _cart;

            decimal total = 0;
            foreach (var c in _cart) total += c.Price * c.Quantity;
            TotalSumText.Text = total.ToString("0.##") + " руб.";
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBoxHelper.Error("Ваша корзина пуста.");
                return;
            }
            var address = AddressTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBoxHelper.Error("Укажите адрес доставки.");
                return;
            }

            try
            {
                var context = App.GetContext();
                var status = context.Status.FirstOrDefault();
                var user = App.CurrentUser ?? context.User.FirstOrDefault();

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    DeliveryAddress = address,
                    Comment = CommentTextBox.Text ?? string.Empty,
                    OrderStatus = status != null ? status.Id : 1,
                    IdUser = user != null ? user.Id : 1,
                    TotalSum = _cart.Sum(c => c.Price * c.Quantity)
                };
                context.Order.Add(order);
                context.SaveChanges();

                foreach (var c in _cart)
                {
                    var oi = new OrderItem
                    {
                        IdOrder = order.Id,
                        IdDish = c.Dish.Id,
                        Quantity = c.Quantity,
                        PriceAtMoment = c.Price
                    };
                    context.OrderItem.Add(oi);
                }
                context.SaveChanges();

                MessageBoxHelper.Information("Заказ успешно создан.");
                _cart.Clear();
                RefreshCart();
                AddressTextBox.Text = string.Empty;
                CommentTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось оформить заказ: " + ex.Message);
            }
        }

        private class CartItem
        {
            public Dish Dish { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}
