using Catering.AppData;
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