using Catering.AppData;
using System;
using System.Windows;

namespace Catering.View.Windows
{
    /// <summary>
    /// Interaction logic for CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        private string _captchaText;
        public bool IsVerified { get; private set; }
        public CaptchaWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }
        private void GenerateCaptcha()
        {
            var random = new Random();
            _captchaText = random.Next(1000, 9999).ToString();
            CaptchaTextBlock.Text = _captchaText;
        }
        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaInput.Text == _captchaText)
            {
                IsVerified = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBoxHelper.Error("Неверный код капчи. Попробуйте еще раз.");
                GenerateCaptcha();
                CaptchaInput.Clear();
            }
        }
    }
}