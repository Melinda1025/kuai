using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// AlarmView.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmView : UserControl
    {
        public AlarmView(AlarmViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
