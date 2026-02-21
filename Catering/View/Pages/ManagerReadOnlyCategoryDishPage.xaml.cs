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

namespace Catering.View.Pages
{
    /// <summary>
    /// Interaction logic for ManagerReadOnlyCategoryDishPage.xaml
    /// </summary>
    public partial class ManagerReadOnlyCategoryDishPage : Page
    {
        public ManagerReadOnlyCategoryDishPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                var context = App.GetContext();
                var categories = context.Category.ToList();
                CategoriesDataGrid.ItemsSource = categories;

                var dishes = context.Dish.ToList();
                DishesDataGrid.ItemsSource = dishes;

                var comboCol = DishesDataGrid.Columns.OfType<DataGridComboBoxColumn>().FirstOrDefault();
                if (comboCol != null)
                    comboCol.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось загрузить данные: " + ex.Message);
            }
        }

        private void CategoriesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction != DataGridEditAction.Commit) return;
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
                if (e.EditAction != DataGridEditAction.Commit) return;
                var context = App.GetContext();
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось сохранить изменения блюда: " + ex.Message);
            }
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = App.GetContext();
                var cat = new Category { Name = "Новая категория" };
                context.Category.Add(cat);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось добавить категорию: " + ex.Message);
            }
        }

        private void DeleteCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = CategoriesDataGrid.SelectedItem as Category;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите категорию для удаления.");
                return;
            }
            try
            {
                var context = App.GetContext();
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
            try
            {
                var context = App.GetContext();
                var firstCat = context.Category.FirstOrDefault();
                var dish = new Dish { Name = "Новое блюдо", Description = "Описание", Price = 0, IdCategory = firstCat != null ? firstCat.Id : 1, IsAvailable = true };
                context.Dish.Add(dish);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error("Не удалось добавить блюдо: " + ex.Message);
            }
        }

        private void DeleteDishBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = DishesDataGrid.SelectedItem as Dish;
            if (selected == null)
            {
                MessageBoxHelper.Error("Выберите блюдо для удаления.");
                return;
            }
            try
            {
                var context = App.GetContext();
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
