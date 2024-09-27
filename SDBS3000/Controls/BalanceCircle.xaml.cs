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
using OxyPlot.Annotations;

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
        private static readonly Brush UnqualifiedBrush;
        private static readonly Brush QualifiedBrush;

        //图像中合格圆与最大半径的比例
        private const double QUALIFIED_RATE = 1 / 0.3;

        static BalanceCircle()
        {
            QualifiedBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            UnqualifiedBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            QualifiedBrush.Freeze();
            UnqualifiedBrush.Freeze();
        }

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

        public double PolarAngle
        {
            get { return (double)GetValue(PolarAngleProperty); }
            set { SetValue(PolarAngleProperty, value); }
        }

        public double PolarRadius
        {
            get { return (double)GetValue(PolarRadiusProperty); }
            set { SetValue(PolarRadiusProperty, value); }
        }

        public double MaxQualifiedRadius
        {
            get { return (double)GetValue(MaxQualifiedRadiusProperty); }
            set { SetValue(MaxQualifiedRadiusProperty, value); }
        }

        public static readonly DependencyProperty MaxQualifiedRadiusProperty =
            DependencyProperty.Register(
                "MaxQualifiedRadius",
                typeof(double),
                typeof(BalanceCircle),
                new PropertyMetadata(30.0, new PropertyChangedCallback(OnMaxQualifiedRadiusChanged))
            );

        private double maxPolarRadius = 100;
        private static void OnMaxQualifiedRadiusChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var circle = (BalanceCircle)d;
            circle.maxPolarRadius = (double)e.NewValue * QUALIFIED_RATE;
            OnUpdatePoint(d, e);
        }

        public static readonly DependencyProperty PolarRadiusProperty = DependencyProperty.Register(
            "PolarRadius",
            typeof(double),
            typeof(BalanceCircle),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnUpdatePoint))
        );

        public static readonly DependencyProperty PolarAngleProperty = DependencyProperty.Register(
            "PolarAngle",
            typeof(double),
            typeof(BalanceCircle),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnUpdatePoint))
        );

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

        private static void OnUpdatePoint(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //未缩放圆的半径
            const double circle_radius = 50;
            //由于点宽度而需要一个偏移
            const double offset = 1;

            var circle = (BalanceCircle)d;

            var maxPolarRadius = circle.maxPolarRadius;
            var polarRadius = circle.PolarRadius;
            //超出最大半径则取最大半径
            var newPolarRadius = Math.Min(polarRadius, maxPolarRadius);

            
            var isQualified = newPolarRadius <= maxPolarRadius / QUALIFIED_RATE;
            var newAngle = (circle.PolarAngle + circle.startAngle) * circle.inverse;

            var newX =
                circle_radius
                + Math.Cos(newAngle / 180 * Math.PI)
                    * newPolarRadius
                    / maxPolarRadius
                    * circle_radius
                - offset;
            var newY =
                circle_radius
                - Math.Sin(newAngle / 180 * Math.PI)
                    * newPolarRadius
                    / maxPolarRadius
                    * circle_radius
                - offset;

            circle.PointTransform.X = newX;
            circle.PointTransform.Y = newY;
            //Canvas.SetLeft(circle, newX);
            //Canvas.SetTop(circle, newY);

            circle.PointInnerBorder.BorderBrush = isQualified ? QualifiedBrush : UnqualifiedBrush;
        }

        public void UpdateText(RotationDirection rotation, ZeroPosition zeroPosition)
        {
            if (rotation == RotationDirection.Clockwise)
            {
                inverse = -1;
                switch (zeroPosition)
                {
                    case ZeroPosition.Up:
                        startAngle = 90;
                        UpTbx.Text = "0°";
                        DownTbx.Text = "180°";
                        LeftTbx.Text = "270°";
                        RightTbx.Text = "90°";
                        break;
                    case ZeroPosition.Down:
                        startAngle = 270;
                        UpTbx.Text = "180°";
                        DownTbx.Text = "0°";
                        LeftTbx.Text = "90°";
                        RightTbx.Text = "270°";
                        break;
                    case ZeroPosition.Left:
                        startAngle = 180;
                        LeftTbx.Text = "0°";
                        RightTbx.Text = "180°";
                        UpTbx.Text = "90°";
                        DownTbx.Text = "270°";
                        break;
                    case ZeroPosition.Right:
                        startAngle = 0;
                        LeftTbx.Text = "180°";
                        RightTbx.Text = "0°";
                        UpTbx.Text = "270°";
                        DownTbx.Text = "90°";
                        break;
                }
            }
            else
            {
                inverse = 1;
                switch (zeroPosition)
                {
                    case ZeroPosition.Up:
                        startAngle = 90;
                        UpTbx.Text = "0°";
                        DownTbx.Text = "180°";
                        LeftTbx.Text = "270°";
                        RightTbx.Text = "90°";
                        break;
                    case ZeroPosition.Down:
                        startAngle = 270;
                        UpTbx.Text = "180°";
                        DownTbx.Text = "0°";
                        LeftTbx.Text = "90°";
                        RightTbx.Text = "270°";
                        break;
                    case ZeroPosition.Left:
                        startAngle = 180;
                        LeftTbx.Text = "0°";
                        RightTbx.Text = "180°";
                        UpTbx.Text = "270°";
                        DownTbx.Text = "90°";
                        break;
                    case ZeroPosition.Right:
                        startAngle = 0;
                        LeftTbx.Text = "180°";
                        RightTbx.Text = "0°";
                        UpTbx.Text = "90°";
                        DownTbx.Text = "270°";
                        break;
                }
            }
            OnUpdatePoint(this, new DependencyPropertyChangedEventArgs());
        }

        private int inverse = 1;
        private int startAngle = 90;

        public BalanceCircle()
        {
            InitializeComponent();
        }
    }
}
