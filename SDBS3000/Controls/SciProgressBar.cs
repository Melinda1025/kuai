using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace SDBS3000.Controls
{
    public class SciProgressBar : FrameworkElement
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public int RectCount
        {
            get { return (int)GetValue(RectCountProperty); }
            set { SetValue(RectCountProperty, value); }
        }

        public static readonly DependencyProperty RectCountProperty = DependencyProperty.Register(
            "RectCount",
            typeof(int),
            typeof(SciProgressBar),
            new PropertyMetadata(10, new PropertyChangedCallback(OnBarChanged))
        );

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(double),
            typeof(SciProgressBar),
            new PropertyMetadata(100.0, new PropertyChangedCallback(OnBarChanged))
        );

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(SciProgressBar),
            new PropertyMetadata(50.0, new PropertyChangedCallback(OnBarChanged))
        );

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(SciProgressBar),
            new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnBarChanged))
        );

        private static void OnBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SciProgressBar).DrawBar();
        }

        private readonly DrawingVisual layer;
        private readonly Brush borderBrush,
            bgBrush,
            darkBrush;
        private readonly Pen borderPen;

        public SciProgressBar()
        {
            borderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 72));
            bgBrush = new SolidColorBrush(Color.FromRgb(6, 37, 65));
            darkBrush = new SolidColorBrush(Color.FromRgb(7, 82, 67));
            borderPen = new Pen(borderBrush, 1);
            borderBrush.Freeze();
            bgBrush.Freeze();
            darkBrush.Freeze();
            borderPen.Freeze();

            layer = new DrawingVisual();
            this.AddVisualChild(layer);
            this.SizeChanged += (s, e) => DrawBar();
            this.Loaded += (s, e) => DrawBar();
        }

        private void DrawBar()
        {
            if (this.ActualHeight <= 0 || this.ActualWidth <= 0) return;
            using var dc = layer.RenderOpen();
            //边框
            dc.DrawRectangle(
                bgBrush,
                borderPen,
                new Rect(0, 0, this.ActualWidth, this.ActualHeight)
            );
            var rate = Value / MaxValue;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var width = (this.ActualWidth - 2) / (1.4 * RectCount + 0.4);
                    var height = (this.ActualHeight - 2 - 0.8 * width);
                    if (height <= 0)
                        height = 1;
                    var startX =
                        (this.ActualWidth - width * RectCount - width * 0.4 * (RectCount - 1)) / 2;

                    for (var i = 0; i < RectCount; i++)
                    {
                        if (i < RectCount * rate)
                        {
                            dc.DrawRectangle(
                                borderBrush,
                                null,
                                new Rect(startX, 1 + 0.4 * width, width, height)
                            );
                        }
                        else
                        {
                            dc.DrawRectangle(
                                darkBrush,
                                null,
                                new Rect(startX, 1 + 0.4 * width, width, height)
                            );
                        }
                        startX += 1.4 * width;
                    }
                    break;
                case Orientation.Vertical:
                    height = (this.ActualHeight - 2) / (1.4 * RectCount + 0.4);
                    width = (this.ActualWidth - 2 - 0.8 * height);
                    if (width <= 0)
                        width = 1;
                    var startY =
                        this.ActualHeight
                        - (this.ActualHeight - height * RectCount - height * 0.4 * (RectCount - 1))
                            / 2 - height;

                    for (var i = 0; i < RectCount; i++)
                    {
                        if (i < RectCount * rate)
                        {
                            dc.DrawRectangle(
                                borderBrush,
                                null,
                                new Rect(1 + 0.4 * height, startY, width, height)
                            );
                        }
                        else
                        {
                            dc.DrawRectangle(
                                darkBrush,
                                null,
                                new Rect(1 + 0.4 * height, startY, width, height)
                            );
                        }
                        startY -= 1.4 * height;
                    }
                    break;
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => layer;
    }
}
