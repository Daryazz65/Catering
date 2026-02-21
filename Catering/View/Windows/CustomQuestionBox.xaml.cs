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
    /// Interaction logic for CustomQuestionBox.xaml
    /// </summary>
    public partial class CustomQuestionBox : Window
    {
        public bool Result { get; private set; }
        public CustomQuestionBox(string message, string title)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            Title = title;
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.DialogResult = true;
            this.Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.DialogResult = false;
            this.Close();
        }
        public static bool Show(string message, string title = "Вопрос")
        {
            CustomQuestionBox questionBox = new CustomQuestionBox(message, title);
            questionBox.ShowDialog();
            return questionBox.Result;
        }
    }
}