using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// SingleStepView.xaml 的交互逻辑
    /// </summary>
    public partial class SingleStepView : UserControl
    {
        public SingleStepView(SingleStepViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
