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
    /// SpacedTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class SpacedTextBlock : ItemsControl
    {
        public SpacedTextBlock()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(SpacedTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged))
        );

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpacedTextBlock block)
            {
                block.ItemsSource = e.NewValue as string;
            }
        }
    }
}
