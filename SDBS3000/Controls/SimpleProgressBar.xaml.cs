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
using HandyControl.Controls;

namespace SDBS3000.Controls
{
    /// <summary>
    /// SimpleProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleProgressBar : SimplePanel
    {
        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public float MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(float),
            typeof(SimpleProgressBar),
            new PropertyMetadata(100f, new PropertyChangedCallback(OnValueChanged))
        );

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(float),
            typeof(SimpleProgressBar),
            new PropertyMetadata(50f, new PropertyChangedCallback(OnValueChanged))
        );

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SimpleProgressBar).DrawBar();
        }

        public SimpleProgressBar()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => DrawBar();
            this.Loaded += (s, e) => DrawBar();
        }

        private void DrawBar()
        {
            var newWidth = this.ActualWidth * Value / MaxValue;
            if (double.IsNaN(newWidth) || newWidth <= 0)
            {
                FillRect.Visibility = Visibility.Collapsed;
                newWidth = 0;
            }
            else
            {
                FillRect.Visibility = Visibility.Visible;
                if(newWidth > this.ActualWidth)
                {
                    newWidth = this.ActualWidth;
                }
            }
            FillRect.Width = newWidth;
        }
    }
}
