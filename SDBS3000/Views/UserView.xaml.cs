using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView(UserViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
