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
using System.Collections.ObjectModel;
using System.Data.Entity;


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

            var allBtn = new Button { Content = "Все", Margin = new Thickness(4,0,4,0), Padding = new Thickness(8,4,8,4) };
            allBtn.Click += (s, e) => RenderDishes(_dishes);
            CategoriesPanel.Children.Add(allBtn);

            foreach (var c in _categories)
            {
                var btn = new Button { Content = c.Name, Tag = c, Margin = new Thickness(4,0,4,0), Padding = new Thickness(8,4,8,4) };
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
            DishesPanel.Children.Clear();
            foreach (var d in dishes)
            {
                var border = new Border { Width = 200, Height = 150, Margin = new Thickness(6), Background = Brushes.White, CornerRadius = new CornerRadius(6), BorderBrush = (Brush)new BrushConverter().ConvertFromString("#ddd"), BorderThickness = new Thickness(1) };
                var sp = new StackPanel { Margin = new Thickness(8) };
                var name = new TextBlock { Text = d.Name, FontWeight = FontWeights.Bold };
                var desc = new TextBlock { Text = d.Description, FontSize = 12, Foreground = Brushes.Gray, TextWrapping = TextWrapping.Wrap, Height = 60 };
                var bottom = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch };
                var price = new TextBlock { Text = d.Price.ToString("0.##") + " руб.", VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Bold };
                var addBtn = new Button { Content = "В корзину", HorizontalAlignment = HorizontalAlignment.Right, Tag = d, Margin = new Thickness(8,0,0,0) };
                addBtn.Click += AddToCart_Click;
                bottom.Children.Add(price);
                bottom.Children.Add(addBtn);

                sp.Children.Add(name);
                sp.Children.Add(desc);
                sp.Children.Add(bottom);
                border.Child = sp;
                DishesPanel.Children.Add(border);
            }
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
            // Refresh ItemsControl
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
