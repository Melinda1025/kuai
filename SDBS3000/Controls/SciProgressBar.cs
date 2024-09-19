using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public Brush LightFillBrush
        {
            get { return (Brush)GetValue(LightFillBrushProperty); }
            set { SetValue(LightFillBrushProperty, value); }
        }

        //public Brush DarkFillBrush
        //{
        //    get { return (Brush)GetValue(DarkFillBrushProperty); }
        //    set { SetValue(DarkFillBrushProperty, value); }
        //}

        public double BorderSize
        {
            get { return (double)GetValue(BorderSizeProperty); }
            set { SetValue(BorderSizeProperty, value); }
        }

        public static readonly DependencyProperty BorderSizeProperty = DependencyProperty.Register(
            "BorderSize",
            typeof(double),
            typeof(SciProgressBar),
            new PropertyMetadata(1.0, new PropertyChangedCallback(OnBarChanged))
        );

        //public static readonly DependencyProperty DarkFillBrushProperty =
        //    DependencyProperty.Register(
        //        "DarkFillBrush",
        //        typeof(Brush),
        //        typeof(SciProgressBar),
        //        new PropertyMetadata(
        //            new SolidColorBrush(Color.FromRgb(7, 82, 67)),
        //            new PropertyChangedCallback(OnBarChanged)
        //        )
        //    );

        public static readonly DependencyProperty LightFillBrushProperty =
            DependencyProperty.Register(
                "LightFillBrush",
                typeof(Brush),
                typeof(SciProgressBar),
                new PropertyMetadata(
                    new SolidColorBrush(Color.FromRgb(0, 201, 255)),
                    new PropertyChangedCallback(OnBarChanged)
                )
            );

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(SciProgressBar),
            new PropertyMetadata(
                new SolidColorBrush(Color.FromRgb(0, 201, 255)),
                new PropertyChangedCallback(OnBorderBrushChanged)
            )
        );

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(SciProgressBar),
            new PropertyMetadata(
                Brushes.Transparent,
                new PropertyChangedCallback(OnBarChanged)
            )
        );

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

        private static void OnBorderBrushChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var bar = (SciProgressBar)d;
            bar.borderPen.Brush = (Brush)e.NewValue;
            bar.DrawBar();
        }

        private readonly DrawingVisual layer;
        private readonly Pen borderPen;

        public SciProgressBar()
        {
            borderPen = new Pen(BorderBrush, BorderSize);
            layer = new DrawingVisual();
            this.AddVisualChild(layer);
            this.SizeChanged += (s, e) => DrawBar();
            this.Loaded += (s, e) => DrawBar();
        }

        private void DrawBar()
        {
            if (this.ActualHeight <= 0 || this.ActualWidth <= 0)
                return;
            using var dc = layer.RenderOpen();            
            //边框
            dc.DrawRoundedRectangle(
                Background,
                borderPen,
                new Rect(0, 0, this.ActualWidth, this.ActualHeight),
                                5,5
            );
            var size = BorderSize;
            var rate = Value / MaxValue;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var width = (this.ActualWidth - 2 * size) / (1.4 * RectCount + 0.4);
                    var height = (this.ActualHeight - 2 * size - 0.8 * width);
                    if (height <= 0)
                        height = 1;
                    var startX =
                        (this.ActualWidth - width * RectCount - width * 0.4 * (RectCount - 1)) / 2;

                    for (var i = 0; i < RectCount; i++)
                    {
                        if (i < RectCount * rate)
                        {
                            dc.DrawRoundedRectangle(
                                BorderBrush,
                                null,
                                new Rect(startX, 1 * size + 0.4 * width, width, height),
                                3, 3
                            );
                        }
                        else break;
                        //else
                        //{
                        //    dc.DrawRoundedRectangle(
                        //        DarkFillBrush,
                        //        null,
                        //        new Rect(startX, 1 * size + 0.4 * width, width, height),
                        //        2, 2
                        //    );
                        //}
                        startX += 1.4 * width;
                    }
                    break;
                case Orientation.Vertical:
                    height = (this.ActualHeight - 2 * size) / (1.4 * RectCount + 0.4);
                    width = (this.ActualWidth - 2 * size - 0.8 * height);
                    if (width <= 0)
                        width = 1;
                    var startY =
                        this.ActualHeight
                        - (this.ActualHeight - height * RectCount - height * 0.4 * (RectCount - 1))
                            / 2
                        - height;

                    for (var i = 0; i < RectCount; i++)
                    {
                        if (i < RectCount * rate)
                        {
                            dc.DrawRoundedRectangle(
                                BorderBrush,
                                null,
                                new Rect(1 * size + 0.4 * height, startY, width, height),
                                3, 3
                            );
                        }
                        else break;
                        //else
                        //{
                        //    dc.DrawRoundedRectangle(
                        //        DarkFillBrush,
                        //        null,
                        //        new Rect(1 * size + 0.4 * height, startY, width, height),
                        //        2, 2
                        //    );
                        //}
                        startY -= 1.4 * height;
                    }
                    break;
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => layer;
    }
}
