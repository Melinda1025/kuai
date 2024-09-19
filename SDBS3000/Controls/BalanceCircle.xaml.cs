using OxyPlot.Annotations;
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
    public enum ZeroPosition
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
    }

    /// <summary>
    /// BalanceCircle.xaml 的交互逻辑
    /// </summary>
    public partial class BalanceCircle : UserControl
    {
        public double AngleSpacing
        {
            get { return (double)GetValue(AngleSpacingProperty); }
            set { SetValue(AngleSpacingProperty, value); }
        }

        public ZeroPosition ZeroPosition
        {
            get { return (ZeroPosition)GetValue(ZeroPositionProperty); }
            set { SetValue(ZeroPositionProperty, value); }
        }

        public RotationDirection RotationDirection
        {
            get { return (RotationDirection)GetValue(RotationDirectionProperty); }
            set { SetValue(RotationDirectionProperty, value); }
        }

        public static readonly DependencyProperty RotationDirectionProperty =
            DependencyProperty.Register(
                "RotationDirection",
                typeof(RotationDirection),
                typeof(BalanceCircle),
                new PropertyMetadata(
                    RotationDirection.CounterClockwise,
                    new PropertyChangedCallback(OnRotationDirectionChanged)
                )
            );

        public static readonly DependencyProperty ZeroPositionProperty =
            DependencyProperty.Register(
                "ZeroPosition",
                typeof(ZeroPosition),
                typeof(BalanceCircle),
                new PropertyMetadata(
                    ZeroPosition.Up,
                    new PropertyChangedCallback(OnZeroPositionChanged)
                )
            );

        public static readonly DependencyProperty AngleSpacingProperty =
            DependencyProperty.Register(
                "AngleSpacing",
                typeof(double),
                typeof(BalanceCircle),
                new PropertyMetadata(10.0, new PropertyChangedCallback(OnAngleSpacingChanged))
            );

        private static void OnAngleSpacingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var circle = (BalanceCircle)d;
            var spacing = (double)e.NewValue;
            circle.UpTbx.Margin = new Thickness(0, 0, 0, spacing);
            circle.DownTbx.Margin = new Thickness(0, spacing, 0, 0);
            circle.LeftTbx.Margin = new Thickness(spacing, 0, 0, 0);
            circle.DownTbx.Margin = new Thickness(0, 0, spacing, 0);
        }

        private static void OnZeroPositionChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var circle = (BalanceCircle)d;
            var position = (ZeroPosition)e.NewValue;
            circle.UpdateText(circle.RotationDirection, position);
        }

        private static void OnRotationDirectionChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var circle = (BalanceCircle)d;
            var rotation = (RotationDirection)e.NewValue;
            circle.UpdateText(rotation, circle.ZeroPosition);
        }

        public void UpdateText(RotationDirection rotation, ZeroPosition zeroPosition)
        {
            if (rotation == RotationDirection.Clockwise)
            {
                switch (zeroPosition)
                {
                    case ZeroPosition.Up:
                        UpTbx.Text = "0°";
                        DownTbx.Text = "180°";
                        LeftTbx.Text = "270°";
                        RightTbx.Text = "90°";
                        break;
                    case ZeroPosition.Down:
                        UpTbx.Text = "180°";
                        DownTbx.Text = "0°";
                        LeftTbx.Text = "90°";
                        RightTbx.Text = "270°";
                        break;
                    case ZeroPosition.Left:
                        LeftTbx.Text = "0°";
                        RightTbx.Text = "180°";
                        UpTbx.Text = "90°";
                        DownTbx.Text = "270°";
                        break;
                    case ZeroPosition.Right:
                        LeftTbx.Text = "180°";
                        RightTbx.Text = "0°";
                        UpTbx.Text = "270°";
                        DownTbx.Text = "90°";
                        break;
                }
            }
            else
            {
                switch (zeroPosition)
                {
                    case ZeroPosition.Up:
                        UpTbx.Text = "0°";
                        DownTbx.Text = "180°";
                        LeftTbx.Text = "270°";
                        RightTbx.Text = "90°";
                        break;
                    case ZeroPosition.Down:
                        UpTbx.Text = "180°";
                        DownTbx.Text = "0°";
                        LeftTbx.Text = "90°";
                        RightTbx.Text = "270°";
                        break;
                    case ZeroPosition.Left:
                        LeftTbx.Text = "0°";
                        RightTbx.Text = "180°";
                        UpTbx.Text = "270°";
                        DownTbx.Text = "90°";
                        break;
                    case ZeroPosition.Right:
                        LeftTbx.Text = "180°";
                        RightTbx.Text = "0°";
                        UpTbx.Text = "90°";
                        DownTbx.Text = "270°";
                        break;
                }
            }
        }

        public BalanceCircle()
        {
            InitializeComponent();            
        }
    }
}
