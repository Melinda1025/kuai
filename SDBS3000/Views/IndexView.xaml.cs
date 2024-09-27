using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public IndexView(IndexViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        private void MinimizeBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
