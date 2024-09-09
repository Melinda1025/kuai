using SDBS3000.ViewModels;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// KeyCompensationView.xaml 的交互逻辑
    /// </summary>
    public partial class KeyCompensationView : UserControl
    {
        public KeyCompensationView(KeyCompensationViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
