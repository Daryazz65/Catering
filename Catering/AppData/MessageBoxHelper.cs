using Catering.View.Windows;
using System;

namespace Catering.AppData
{
    internal class MessageBoxHelper
    {
        public static void Error(Exception exception)
        {
            CustomMessageBox.Show(exception.Message, exception.HelpLink);
        }
        public static void Error(string text, string title = "Ошибка")
        {
            CustomMessageBox.Show(text, title);
        }
        public static void Information(string text, string title = "Информация")
        {
            CustomMessageBox.Show(text, title);
        }
        public static bool Question(string text, string title = "Вопрос")
        {
            return CustomQuestionBox.Show(text, title);
        }
        public static void Warning(string text, string title = "Предупреждение")
        {
            CustomMessageBox.Show(text, title);
        }
    }
}