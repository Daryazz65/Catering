using Catering.AppData;
using Catering.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for AdminCrudCategoryDishPage.xaml
    /// </summary>
    public partial class AdminCrudCategoryDishPage : Page
    {
        public AdminCrudCategoryDishPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void CategoriesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения категории: " + ex.Message);
            }
        }

        private void DishesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения блюда: " + ex.Message);
            }
        }

        private void LoadData()
        {
            var context = App.GetContext();
            var categories = context.Category.ToList();
            CategoriesDataGrid.ItemsSource = categories;

            var dishes = context.Dish.ToList();
            DishesDataGrid.ItemsSource = dishes;

            var comboCol = DishesDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault();
            if (comboCol != null)
            {
                comboCol.ItemsSource = categories;
            }
        }

        private void RefreshAll()
        {
            LoadData();
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var context = App.GetContext();
            var cat = new Category { Name = "Новая категория" };
            context.Category.Add(cat);
            context.SaveChanges();
            LoadData();
        }

        private void DeleteCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = CategoriesDataGrid.SelectedItem as Category;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите категорию для удаления.");
                return;
            }
            var context = App.GetContext();
            try
            {
                var toDelete = context.Category.First(c => c.Id == selected.Id);
                context.Category.Remove(toDelete);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось удалить категорию: " + ex.Message);
            }
        }

        private void AddDishBtn_Click(object sender, RoutedEventArgs e)
        {
            var context = App.GetContext();
            var firstCat = context.Category.FirstOrDefault();
            var dish = new Dish { Name = "Новое блюдо", Description = "Описание", Price = 0, IdCategory = firstCat != null ? firstCat.Id : 1, IsAvailable = true };
            context.Dish.Add(dish);
            context.SaveChanges();
            LoadData();
        }

        private void DeleteDishBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = DishesDataGrid.SelectedItem as Dish;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите блюдо для удаления.");
                return;
            }
            var context = App.GetContext();
            try
            {
                var toDelete = context.Dish.First(d => d.Id == selected.Id);
                context.Dish.Remove(toDelete);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось удалить блюдо: " + ex.Message);
            }
        }
    }
}
