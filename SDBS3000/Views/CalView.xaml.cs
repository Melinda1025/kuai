using SDBS3000.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
            switch ((sender as RadioButton).Tag)
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
