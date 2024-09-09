using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// RotorView.xaml 的交互逻辑
    /// </summary>
    public partial class RotorView : UserControl
    {
        public RotorView(RotorViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
