using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDBS3000.Views.Dialogs
{
    /// <summary>
    /// SelectSupportModeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectSupportModeDialog : UserControl
    {
        public SelectSupportModeDialog(int simulationMode)
        {
            InitializeComponent();
            if(simulationMode == 1)
            {
                SupportMode2.Visibility = Visibility.Collapsed;
                SupportMode4.Visibility = Visibility.Collapsed;
                SupportMode5.Visibility = Visibility.Collapsed;
                SupportMode7.Visibility = Visibility.Collapsed;
                SupportMode8.Visibility = Visibility.Collapsed;
                SupportMode9.Visibility = Visibility.Collapsed;
                SupportMode10.Visibility = Visibility.Collapsed;                                
            }
        }
    }
}
