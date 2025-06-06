using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CalculatorApp.ViewModels;

namespace CalculatorApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += MainWindow_SourceInitialized;
        }
        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            WindowUtil.SetImmersiveDarkMode(Application.Current.MainWindow, true);  // Apply dark mode initially
        }
    }
}