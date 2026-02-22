using Catering.AppData;
using System;
using System.Linq;
using System.Windows.Controls;

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

    }
}
