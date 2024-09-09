using SDBS3000.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
