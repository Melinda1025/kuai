using SDBS3000.ViewModels.Dialogs;
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
    /// AuthorisationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorisationDialog : UserControl
    {
        public AuthorisationDialog(AuthorisationViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
