using System.Windows;
using System.Windows.Controls;

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
