using SDBS3000.ViewModels.Dialogs;
using System.Windows.Controls;

namespace SDBS3000.Views.Dialogs
{
    /// <summary>
    /// AxisParamsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AxisParamsDialog : UserControl
    {
        public AxisParamsDialog(AxisParamsViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
