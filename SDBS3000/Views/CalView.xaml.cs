using SDBS3000.ViewModels;
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

namespace SDBS3000.Views
{
    /// <summary>
    /// CalView.xaml 的交互逻辑
    /// </summary>
    public partial class CalView : UserControl
    {
        public CalView(CalViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }

        private void CalRbtn_Checked(object sender, RoutedEventArgs e)
        {
            SelectorBorder.Visibility = Visibility.Visible;
            switch((sender as RadioButton).Tag)
            {
                case "1":
                    Arrow1.Visibility = Visibility.Visible;
                    Arrow2.Visibility = Visibility.Collapsed;
                    Grid.SetRow(SelectorBorder, 0);
                    break;
                case "2":
                    Arrow1.Visibility = Visibility.Collapsed;
                    Arrow2.Visibility = Visibility.Visible;
                    Grid.SetRow(SelectorBorder, 1);
                    break;
                case "3":
                    Arrow1.Visibility = Visibility.Collapsed;
                    Arrow2.Visibility = Visibility.Collapsed;
                    Grid.SetRow(SelectorBorder, 2);
                    break;
            }
        }
    }
}
