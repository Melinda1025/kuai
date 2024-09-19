using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// KeyView.xaml 的交互逻辑
    /// </summary>
    public partial class KeyView : UserControl
    {
        public KeyView(KeyViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
