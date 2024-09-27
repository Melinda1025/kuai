using HandyControl.Controls;
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

namespace SDBS3000.Controls
{
    /// <summary>
    /// GrowText.xaml 的交互逻辑
    /// </summary>
    public partial class GrowText : UserControl
    {


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GrowText), new PropertyMetadata(string.Empty));


        public GrowText()
        {
            InitializeComponent();
        }
    }
}
