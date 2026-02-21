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
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message, string title)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            Title = title;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        public static void Show(string message, string title = "Сообщение")
        {
            CustomMessageBox messageBox = new CustomMessageBox(message, title);
            messageBox.ShowDialog();
        }
    }
}