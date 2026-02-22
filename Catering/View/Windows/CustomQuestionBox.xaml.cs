using System.Windows;

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