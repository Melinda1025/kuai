using SDBS3000.ViewModels;
using System.Windows;

namespace SDBS3000.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
