using Catering.Model;
using System.Windows;

namespace Catering
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
        private static SyleymanovaEntities _context;
        public static SyleymanovaEntities GetContext()
        {
            if (_context == null)
            {
                _context = new SyleymanovaEntities();
            }
            return _context;
        }
    }
}