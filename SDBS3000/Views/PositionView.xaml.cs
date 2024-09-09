using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// PositionView.xaml 的交互逻辑
    /// </summary>
    public partial class PositionView : UserControl
    {
        public PositionView(PositionViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
